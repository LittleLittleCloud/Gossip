using Gossip.Core;
using Gossip.Web.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gossip.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicController : ControllerBase
    {
        private TopicContext topic;

        public TopicController(TopicContext topicContext)
        {
            this.topic = topicContext;
        }

        [HttpGet]
        [Route("all")]
        public IEnumerable<Topic> GetAllTopics()
        {
            return this.topic.Topics;
        }

        [HttpPost]
        [Route("{topicId}")]
        public void CreateTopic(long topicId)
        {
            var topic = new Topic()
            {
                ID = topicId,
            };

            this.topic.Topics.Add(topic);
            this.topic.SaveChanges();
        }
    }
}
