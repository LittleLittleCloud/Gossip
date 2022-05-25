using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class Reply
    {
        public long ID { get; set; }

        public long TimeStamp { get; set; }

        public string Content { get; set; }

        public User CreatedBy { get; set; }
    }
}
