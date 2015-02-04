using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Models
{
    public class TwitterTweet
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string TwitterId { get; set; }
        public string Tweet { get; set; }
        public string TweetUrl { get; set; }
        public string Response { get; set; }
        public string ResponseUrl { get; set; }
        public DateTime? TweetTimestamp {get;set;}
        public DateTime? ResponseTimestamp { get; set; }
    }
}
