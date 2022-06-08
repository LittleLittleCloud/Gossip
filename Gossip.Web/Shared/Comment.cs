using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class Comment
    {
        public long ID { get; set; }

        public long CreatedTimestampInSeconds { get; set; }

        public string Content { get; set; }

        public User CreatedBy { get; set; }

        public Topic Topic { get; set; }

        public int Section { get; set; }
    }
}
