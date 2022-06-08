using Gossip.Core;
using Gossip.Web.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gossip.Web.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TopicController : ControllerBase
    {
        private GossipContext gossip;

        public TopicController(GossipContext topicContext)
        {
            this.gossip = topicContext;
        }

        /// <summary>
        /// get topic with <paramref name="topicId"/>, create a new topic if not exist.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Produces("application/json")]
        public async Task<Topic> GetOrCreateTopic(string topicId)
        {
            if(this.gossip.Find<Topic>(topicId) is Topic topic)
            {
                return topic;
            }
            else
            {
                topic = new Topic()
                {
                    ID = topicId,
                    CreatedTimestampInSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                };

                this.gossip.Add(topic);
                await this.gossip.SaveChangesAsync();

                return topic;
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> RemoveTopic(string topicId)
        {
            if (this.gossip.Find<Topic>(topicId) is Topic topic)
            {
                this.gossip.Remove(topic);
                await this.gossip.SaveChangesAsync();

                return this.Ok($"remove {topicId} successfully");
            }
            else
            {
                return this.Ok($"{topicId} not exist");
            }
        }


        [HttpGet]
        [Route("comments")]
        [Produces("application/json")]
        public async Task<IActionResult> GetComments(string topicId, int section)
        {
            var topic = await this.gossip.Topics.FindAsync(topicId);
            if (topic is null)
            {
                return this.NotFound($"can't find topic with {topicId}");
            }

            var comments = this.gossip.Comments.Include(c => c.CreatedBy).Where(c => c.Topic.ID == topicId && c.Section == section);

            foreach(var comment in comments)
            {
                comment.CreatedBy = new User
                {
                    Id = comment.CreatedBy.Id,
                    AvatarUrl = comment.CreatedBy.AvatarUrl,
                };
            };

            return this.Ok(comments);
        }

        [HttpGet]
        [Route("comments/count")]
        public async Task<IActionResult> GetCommentsCount(string topicId)
        {
            var topic = await this.gossip.Topics.FindAsync(topicId);
            if (topic is null)
            {
                return this.NotFound($"can't find topic with {topicId}");
            }

            var commentsCount = this.gossip.Comments.Where(c => c.Topic.ID == topicId).Count();
            return this.Ok(commentsCount);
        }
    }
}
