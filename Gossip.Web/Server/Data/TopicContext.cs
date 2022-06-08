using Duende.IdentityServer.EntityFramework.Options;
using Gossip.Core;
using Gossip.Web.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gossip.Web.Server.Data
{
    public class GossipContext : ApiAuthorizationDbContext<User>
    {
        public GossipContext(DbContextOptions<GossipContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Reply> Replies { get; set; }

        public DbSet<Like> Reacts { get; set; }
    }
}
