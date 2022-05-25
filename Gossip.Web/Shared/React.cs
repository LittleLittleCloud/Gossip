using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class React
    {
        public long ID { get; set; }

        public long TimeStamp { get; set; }

        public bool IsThumbUp { get; set; }

        public User CreatedBy { get; set; }
    }
}
