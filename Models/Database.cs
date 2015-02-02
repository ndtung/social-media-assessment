using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MCIFramework.Models;


namespace MCIFramework
{
    public class Database : DbContext
    {
        public DbSet<Assessment> assessments { get; set; }

        public DbSet<SocialMediaStat> socialMediaStats { get; set; }
        public DbSet<FacebookPost> facebookPosts { get; set; }
        public DbSet<TwitterTweet> twitterTweets { get; set; }
    }
    
}
