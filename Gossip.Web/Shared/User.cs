using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class User: IdentityUser, IEquatable<User>
    {
        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public bool Equals(User? other)
        {
            if(other is User u)
            {
                return u.Id == this.Id;
            }
            else
            {
                return false;
            }
        }
    }
}
