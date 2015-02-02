using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using Facebook;

namespace MCIFramework.Helper
{
    public class FacebookImporter
    {
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private string _facebookId;
        public FacebookImporter(Assessment assessment, SocialMediaStat socialMediaStat)
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
         //
            
            var client = new FacebookClient("34a1fd1a479d5fe0347e156f4cb7d2cf");
            client.AppId = "781330755269938";
            client.AppSecret = "c2be2a447122fd7818fdcf0413ebc2";
            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = "781330755269938",
                client_secret = "89c2be2a447122fd7818fdcf0413ebc2",
                grant_type = "read_stream"
            });

            //var apptoken = result.access_token;

            //Facebook.JsonObject me = (Facebook.JsonObject)client.Get("access_token?client_id=781330755269938&client_secret=c2be2a447122fd7818fdcf0413ebc2&grant_type=client_credentials"  );
            //Facebook.JsonObject me = (Facebook.JsonObject)client.Get("183306468352624");
            //String a = me["name"].ToString();
            Facebook.JsonObject posts = (Facebook.JsonObject)client.Get("183306468352624/posts");

        }
    }
}
