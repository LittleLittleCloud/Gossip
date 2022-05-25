using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class Comment
    {
        public long ID { get; set; }

        public long Timestamp { get; set; }

        public string Content { get; set; }

        public User CreatedBy { get; set; }

        public IEnumerable<React> Reacts { get; set; }

        public IEnumerable<Reply> Replys { get; set; }
    }
}
