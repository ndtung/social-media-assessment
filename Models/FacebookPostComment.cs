using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Models
{
    public class FacebookPostComment
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string FacebookId { get; set; }
        public string Post { get; set; }
        public string PostUrl { get; set; }
        public string PostTimestamp { get; set; }
        public string PostComment { get; set; }
        public string Response { get; set; }
        public string ResponseTime { get; set; }
    }
}
