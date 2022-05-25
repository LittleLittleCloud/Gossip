using Duende.IdentityServer.EntityFramework.Options;
using Gossip.Core;
using Gossip.Web.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gossip.Web.Server.Data
{
    public class TopicContext : DbContext
    {
        public TopicContext(DbContextOptions<TopicContext> options)
            : base(options)
        {
        }

        public DbSet<Topic> Topics { get; set; }
    }
}
