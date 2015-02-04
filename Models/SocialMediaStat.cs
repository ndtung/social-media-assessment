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
        public int? FacebookLikes { get; set; }
        public int? FacebookFans { get; set; }
        public int? FacebookPosts { get; set; }
        public int? FacebookComments { get; set; }
        public int? FacebookShares { get; set; }

        public int? TwitterFollowers { get; set; }
        public int? TwitterTweets { get; set; }
        public int? TwitterFavourites { get; set; }
        public int? TwitterReplies { get; set; }
        public int? TwitterRetweets { get; set; }
    }
}
