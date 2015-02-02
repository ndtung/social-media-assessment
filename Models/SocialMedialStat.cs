using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Models
{
    public class SocialMediaStat
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public int TotalDays { get; set; }
        public string YoutubeSubscribers { get; set; }
        public string YoutubeVideos { get; set; }
        public string YoutubeVideoViews { get; set; }
        public string YoutubeVideoLikes { get; set; }
        public string YoutubeVideoDislikes { get; set; }
    }
}
