using Gossip.Core;
using Gossip.Web.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gossip.Web.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CommentController : Controller
    {
        private GossipContext gossip;

        public CommentController(GossipContext context)
        {
            this.gossip = context;
        }

        [HttpPost]
        [Route("create")]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> CreateCommentAsync(string content, string topicId)
        {
            var userId = this.User.Identity!.Name;
            var topic = await this.gossip.Topics.FindAsync(topicId);
            var user = await this.gossip.Users.FindAsync(userId);

            if(topic == null)
            {
                return this.NotFound($"topic {topicId} not exist");
            }

            if(user == null)
            {
                return this.NotFound($"user {userId} not exist");
            }

            if (string.IsNullOrEmpty(content))
            {
                return this.StatusCode(404, "content can't be empty");
            }

            if(content.Length >= 3000)
            {
                return this.StatusCode(404, "content too long");
            }

            var comment = new Comment
            {
                Content = content,
                CreatedBy = user,
                Topic = topic,
                CreatedTimestampInSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Section = 0,
            };

            await this.gossip.Comments.AddAsync(comment);
            await this.gossip.SaveChangesAsync();

            return this.Ok(comment);
        }

        [HttpGet]
        [Route("delete")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteCommentAsync(long commentId)
        {
            var comment = await this.gossip.Comments.FindAsync(commentId);
            if(comment is Comment)
            {
                this.gossip.Comments.Remove(comment);
                await this.gossip.SaveChangesAsync();

                return this.Ok(comment);
            }

            return this.NotFound($"comment {commentId} not found");
        }

        [HttpGet]
        [Route("replies")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRepliesAsync(long commentId, int section)
        {
            var comment = await this.gossip.Comments.FindAsync(commentId);
            if(comment is null)
            {
                return this.NotFound($"comment {commentId} not found");
            }

            var replies = this.gossip.Replies
                            .Include(r => r.CreatedBy)
                            .Where(r => r.Comment.ID == commentId && r.Section == section);

            foreach (var reply in replies)
            {
                reply.CreatedBy = new User
                {
                    Id = reply.CreatedBy.Id,
                    AvatarUrl = reply.CreatedBy.AvatarUrl,
                };
            };
            return this.Ok(replies);
        }

        [HttpGet]
        [Route("replies/count")]
        public async Task<IActionResult> GetRepliesCount(long commentId)
        {
            var comment = await this.gossip.Comments.FindAsync(commentId);
            if (comment is null)
            {
                return this.NotFound($"comment {commentId} not found");
            }

            var repliesCount = this.gossip.Replies.Where(c => c.Comment.ID == commentId).Count();
            return this.Ok(repliesCount);
        }

        [HttpPost]
        [Authorize]
        [Route("upvote/create")]
        public async Task<IActionResult> CreateUpvoteAsync(long commentId)
        {
            var userId = this.User.Identity!.Name;
            var comment = await this.gossip.Comments.FindAsync(commentId);
            var user = await this.gossip.Users.FindAsync(userId);

            if (comment is null)
            {
                return this.NotFound($"comment {commentId} not found");
            }

            if (user == null)
            {
                return this.NotFound($"user {userId} not exist");
            }

            if(this.gossip.Reacts.Where(r =>r.IsLike == true && r.CreatedBy.Id == userId && r.Comment.ID == commentId).Count() > 0)
            {
                return this.Ok("already exist");
            }

            var upvote = new Like
            {
                Comment = comment,
                CreatedBy = user,
                CreatedTimestampInSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsLike = true,
            };

            await this.gossip.Reacts.AddAsync(upvote);
            await this.gossip.SaveChangesAsync();

            return this.Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("upvote/delete")]
        public async Task<IActionResult> DeleteUpvoteAsync(long commentId)
        {
            var userId = this.User.Identity!.Name;
            var comment = await this.gossip.Comments.FindAsync(commentId);
            var user = await this.gossip.Users.FindAsync(userId);

            if (comment is null)
            {
                return this.NotFound($"comment {commentId} not found");
            }

            if (user == null)
            {
                return this.NotFound($"user {userId} not exist");
            }

            if (this.gossip.Reacts.Where(r => r.IsLike == true && r.CreatedBy.Id == userId && r.Comment.ID == commentId).First() is Like like)
            {
                this.gossip.Reacts.Remove(like);
                await this.gossip.SaveChangesAsync();
                return this.Ok();
            }

            return this.Ok("not exist");
        }

        [HttpPost]
        [Route("upvote/count")]
        public async Task<IActionResult> CountUpvoteAsync(long commentId)
        {
            var comment = await this.gossip.Comments.FindAsync(commentId);

            if (comment is null)
            {
                return this.NotFound($"comment {commentId} not found");
            }

            var count = this.gossip.Reacts.Where(r => r.IsLike == true && r.Comment.ID == commentId).Count();

            return this.Ok(count);
        }
    }
}
