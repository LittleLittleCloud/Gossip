using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class Like
    {
        public long ID { get; set; }

        public long CreatedTimestampInSeconds { get; set; }

        public bool IsLike { get; set; }

        public User CreatedBy { get; set; }

        public Comment Comment { get; set; }
    }
}
