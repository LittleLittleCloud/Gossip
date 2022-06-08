using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gossip.Core
{
    public class Topic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }

        public long CreatedTimestampInSeconds { get; set; }
    }
}
