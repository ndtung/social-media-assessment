using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using Facebook;
using Newtonsoft.Json;
using System.Globalization;

namespace MCIFramework.Helper
{
    public class FacebookImporter
    {
        private int _totalLikes = 0;
        private int _totalComments = 0;
        private int _totalPosts = 0;
        private int _totalShares = 0;
        private int _totalFans = 0;
        private string _pageId;
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private FacebookClient _client;
        private int _maximumPostComment = Int32.Parse(Properties.Resources._api_facebook_maximum_comments_to_retrieve_of_a_post);
        public FacebookImporter(Assessment assessment, SocialMediaStat socialMediaStat)
        {
            _assessment = assessment;
            _socialMediaStat = socialMediaStat;
            string accesstoken;
            Database db = new Database();
            var access = db.apis.Where(x => x.Name == "FBAccessToken").FirstOrDefault();
            if (access != null)
            {
                accesstoken = access.Value;
                _client = new FacebookClient(accesstoken);
                _client.AppId = Properties.Resources._api_facebook_app_id;
                _client.AppSecret = Properties.Resources._api_facebook_app_secret;
            }
            else
            {
                _client = new FacebookClient();
            }
        }

        public void Process()
        {
            try
            {
                GetPageId();
                List<FacebookPost> actualPosts = GetFacebookPosts();
                GetAllComments(actualPosts);
                GetFirstReplyFromPage(actualPosts);
                // Find photo albulms
                SaveToDB(actualPosts);
                SaveToExcel(actualPosts);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void GetPageId()
        {
            var info = _client.Get(_assessment.FacebookUsername + "/?fields=id,likes").ToString();
            FacebookPage pageInfo = JsonConvert.DeserializeObject<FacebookPage>(info);
            _pageId = pageInfo.id;
            _totalFans = pageInfo.likes;
        }

        private List<FacebookPost> GetFacebookPosts()
        {
            List<FacebookPost> pagePosts = new List<FacebookPost>();
            // Getting all page posts after assessment start date
            var statuses = _client.Get(_assessment.FacebookUsername+"/posts?fields=id,actions,message,shares,comments.summary(true),likes.summary(true)").ToString();
            FacebookPosts result = JsonConvert.DeserializeObject<FacebookPosts>(statuses);
            pagePosts.AddRange(result.data);

            if (result.paging != null && result.paging.next != null)
            {
                FacebookPosts pageResult;
                string nextRequest = result.paging.next;
                do
                {
                    var pageStatus = _client.Get(nextRequest).ToString();
                    pageResult = JsonConvert.DeserializeObject<FacebookPosts>(pageStatus);
                    pagePosts.AddRange(pageResult.data);
                    if (pageResult.paging != null && pageResult.paging.next != null)
                        nextRequest = pageResult.paging.next;
                    else
                        break;
                }
                while
                (
                    ConvertToDateTime(pageResult.data[pageResult.data.Count - 1].created_time) >= _assessment.StartDate
                );
            }
            // Filter actual page posts, which have a message in it.
            List<FacebookPost> actualPosts = pagePosts.Where(p => p.message != null && ConvertToDateTime(p.created_time) <= _assessment.EndDate && ConvertToDateTime(p.created_time) >= _assessment.StartDate).ToList();
            return actualPosts;
        }

        private void GetAllComments(List<FacebookPost> posts)
        {
            if (posts.Count != 0)
                _totalPosts = posts.Count;
            else
                _totalPosts = 0;
            foreach (FacebookPost post in posts)
            {
                if (post.comments != null)
                {
                    FacebookComments commentGroups;
                    if (post.comments.paging != null && post.comments.paging.next != null)
                    {
                        string nextRequest = post.comments.paging.next;
                        do
                        {
                            var nextCommentGroups = _client.Get(nextRequest).ToString();
                            commentGroups = JsonConvert.DeserializeObject<FacebookComments>(nextCommentGroups);
                            post.comments.data.AddRange(commentGroups.data);
                            if (commentGroups.paging != null && commentGroups.paging.next != null)
                                nextRequest = commentGroups.paging.next;
                            else
                                break;
                        }
                        while (true && post.comments.data.Count < _maximumPostComment);// Get all maximum comments
                    }
                    if (post.shares!=null)
                        _totalShares += post.shares.count;
                    if (post.likes!=null)
                        _totalLikes += post.likes.data.Count;
                    if (post.comments!=null)
                        _totalComments += post.comments.data.Count;
                    post.comments.data.OrderBy(x => x.created_time);
                }
            }
            
        }

        private void GetFirstReplyFromPage(List<FacebookPost> posts)
        {
            foreach (FacebookPost post in posts)
            {
                if (post.comments != null)
                {
                    foreach (FacebookComment comment in post.comments.data)
                    {
                        // Get all replies 
                        List<FacebookComment> allReplies = new List<FacebookComment>();
                        var data = _client.Get(comment.id + "/comments").ToString();
                        FacebookComments commentReplies = JsonConvert.DeserializeObject<FacebookComments>(data);
                        if (commentReplies.data != null)
                        {
                            allReplies.AddRange(commentReplies.data.ToList());
                            FacebookComments nextCommentGroupReplies;
                            if (commentReplies.paging != null && commentReplies.paging.next != null)
                            {
                                string nextRequest = commentReplies.paging.next;
                                do
                                {
                                    var nextCommentGroups = _client.Get(nextRequest).ToString();
                                    nextCommentGroupReplies = JsonConvert.DeserializeObject<FacebookComments>(nextCommentGroups);
                                    allReplies.AddRange(nextCommentGroupReplies.data.ToList());
                                    if (nextCommentGroupReplies.paging != null && nextCommentGroupReplies.paging.next != null)
                                        nextRequest = nextCommentGroupReplies.paging.next;
                                    else
                                        break;
                                }
                                while (true);// Get all replies
                            }
                        }
                        
                        //At this moment, all replies are stored in allReplies
                        var firstPageReply = allReplies.OrderBy(x => x.created_time).FirstOrDefault(x => x.from.id == _pageId);
                        if (firstPageReply != null)
                        {
                            comment.first_reply = firstPageReply.message;
                            comment.first_reply_time = firstPageReply.created_time;
                        }
                    }
                }
            }
        }

        private DateTime ConvertToDateTime(String jsonDateTime)
        {
            return DateTime.Parse(jsonDateTime, null, DateTimeStyles.RoundtripKind);
        }

        public SocialMediaStat GetDataToStore()
        {
            _socialMediaStat.FacebookComments = _totalComments;
            _socialMediaStat.FacebookFans = _totalFans;
            _socialMediaStat.FacebookLikes = _totalLikes;
            _socialMediaStat.FacebookPosts = _totalPosts;
            _socialMediaStat.FacebookShares = _totalShares;
            return _socialMediaStat;
        }

        private void SaveToExcel(List<FacebookPost> posts)
        {

        }

        private void SaveToDB(List<FacebookPost> posts)
        {
            Database context = new Database();
            // clear all existing postComment of this assessment
            context.facebookPostComments.RemoveRange(context.facebookPostComments.Where(x=>x.AssessmentId == _assessment.Id));
            foreach(FacebookPost post in posts)
            {
                if (post.comments != null)
                {
                    foreach (FacebookComment comment in post.comments.data)
                    {
                        FacebookPostComment facebookPostComment = new FacebookPostComment();
                        facebookPostComment.AssessmentId = _assessment.Id;
                        facebookPostComment.FacebookId = _pageId;
                        facebookPostComment.Post = post.message;
                        facebookPostComment.PostComment = comment.message;
                        facebookPostComment.PostTimestamp = post.created_time;
                        facebookPostComment.PostUrl = post.actions[0].link;
                        if (comment.first_reply != null)
                        {
                            facebookPostComment.Response = comment.first_reply;
                            facebookPostComment.ResponseTime = comment.first_reply_time;
                        }
                        context.facebookPostComments.Add(facebookPostComment);
                    }
                }
                else
                {
                    FacebookPostComment facebookPostComment = new FacebookPostComment();
                    facebookPostComment.AssessmentId = _assessment.Id;
                    facebookPostComment.FacebookId = _pageId;
                    facebookPostComment.Post = post.message;
                    facebookPostComment.PostTimestamp = post.created_time;
                    facebookPostComment.PostUrl = post.actions[0].link;
                    context.facebookPostComments.Add(facebookPostComment);
                }
            }
            context.SaveChanges();
        }
       
    }

    public class FacebookPosts
    {
        public List<FacebookPost> data { get; set; }
        public Paging paging { get; set; }
    }

    public class FacebookPost
    {
        public string id{ get;set;}
        public string message {get;set;}
        public FacebookComments comments { get; set; }
        public FacebookLikes likes {get;set;}
        public Share shares {get;set;}
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public List<Action> actions { get; set; }
    }
    
    public class FacebookComments
    {
        public List<FacebookComment> data { get; set; }
        public LikePaging paging { get; set; }
        public Summary summary { get; set; }
    }

    public class FacebookComment
    {
        public string id { get; set; }
        public string message { get; set; }
        public string created_time { get; set; }
        public string first_reply { get; set; }
        public string first_reply_time { get; set; }
        public From from { get; set; }
    }

    public class FacebookLikes
    {
        public List<FacebokLike> data { get; set; }
        public LikePaging paging { get; set; }
        public Summary summary { get; set; }
    }
    
    public class FacebokLike
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Summary
    {
        public string total_count { get; set; }
        public string order { get; set; }
    }

    public class Paging
    {
        public string previous { get; set; }
        public string next{ get; set;}
    }

    public class LikePaging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
    }

    public class Cursors
    {
        public string after { get; set; }
        public string before { get; set; }
    }

    public class Share
    {
        public int count { get; set; }
    }

    public class FacebookPage
    {
        public string id { get; set; }
        public int likes {get;set;}
    }

    public class From
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Action
    {
        public string name { get; set; }
        public string link { get; set; }
    }
}
