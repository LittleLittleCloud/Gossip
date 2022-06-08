using Gossip.Core;
using Gossip.Web.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gossip.Web.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReplyController : Controller
    {
        private GossipContext gossip;

        public ReplyController(GossipContext context)
        {
            this.gossip = context;
        }

        [HttpPost]
        [Route("create")]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> CreateReplyAsync(string content, long commentId)
        {
            var userId = this.User.Identity!.Name;
            var comment = await this.gossip.Comments.FindAsync(commentId);
            var user = await this.gossip.Users.FindAsync(userId);

            if (comment == null)
            {
                return this.NotFound($"comment {commentId} not exist");
            }

            if (user == null)
            {
                return this.NotFound($"user {userId} not exist");
            }

            if (string.IsNullOrEmpty(content))
            {
                return this.StatusCode(404, "content can't be empty");
            }

            if (content.Length >= 3000)
            {
                return this.StatusCode(404, "content too long");
            }

            var reply = new Reply
            {
                Content = content,
                CreatedBy = user,
                Comment = comment,
                CreatedWhenTimestampInSecond = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Section = 0,
            };

            await this.gossip.Replies.AddAsync(reply);
            await this.gossip.SaveChangesAsync();

            return this.Ok(reply);
        }

        [HttpGet]
        [Route("delete")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteReplyAsync(long replyId)
        {
            var reply = await this.gossip.Replies.FindAsync(replyId);
            if (reply is Reply)
            {
                this.gossip.Replies.Remove(reply);
                await this.gossip.SaveChangesAsync();

                return this.Ok(reply);
            }

            return this.NotFound($"reply {replyId} not found");
        }
    }
}
