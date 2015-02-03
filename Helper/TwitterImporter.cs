using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;

namespace MCIFramework.Helper
{
    public class TwitterImporter
    {
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private string _twitterUsername;
        public TwitterImporter(Assessment assessment, SocialMediaStat socialMediaStat)
        {
            _assessment = assessment;
            _socialMediaStat = socialMediaStat;
        }
        public Boolean Start()
        {
            try
            {
                Process();
                return true;
            }
            catch
            {
                return false;
            }

        }
        private void Process()
        {

        }

        public SocialMediaStat GetDataToStore()
        {
            return _socialMediaStat;
        }
    }
}
