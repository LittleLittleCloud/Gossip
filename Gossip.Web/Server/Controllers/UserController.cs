using Gossip.Core;
using Gossip.Web.Server.Data;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Gossip.Web.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private GossipContext gossip;
        private byte[] salt = new byte[32];

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }


        public UserController(IClientRequestParametersProvider clientRequestParametersProvider, GossipContext topicContext)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            this.gossip = topicContext;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> CreateUser(string nickName, string email, string password)
        {
            var user = await this.gossip.Users.FindAsync(nickName);
            if(user != null)
            {
                return this.StatusCode(404, "user exist");
            }

            user = new User()
            {
                Id = nickName,
                Email = email,
                PasswordHash = HashPassword(password),
            };

            await this.gossip.Users.AddAsync(user);

            await this.gossip.SaveChangesAsync();
            return this.Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> GetUserAsync(string nickName, string password)
        {
            var user = await this.gossip.Users.FindAsync(nickName);
            if (user is User)
            {
                if(VerifyHashedPassword(user.PasswordHash, password))
                {
                    var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, "IdentityServerSPA");
                    return Ok(parameters);
                }
                else
                {
                    return this.Forbid();
                }
            }
            else
            {
                return this.Forbid();
            }
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
