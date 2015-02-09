﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using Facebook;
using Newtonsoft.Json;
using System.Globalization;
using System.Data.OleDb;
using System.IO;
using System.Data;
using OfficeOpenXml;
using System.Reflection;


namespace MCIFramework.Helper
{
    public class FacebookImporter
    {
        private int _rowPerTime = 100;
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
                Database db = new Database();
                SaveToExcel(db.facebookPostComments.Where(x => x.AssessmentId == _assessment.Id).ToList());
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
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
            var statuses = _client.Get(_assessment.FacebookUsername + "/posts?fields=id,actions,message,shares,comments.summary(true),likes.summary(true)").ToString();
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
                    if (post.shares != null)
                        _totalShares += post.shares.count;
                    if (post.likes != null)
                        _totalLikes += post.likes.data.Count;
                    if (post.comments != null)
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

        private string ConvertToJson(DateTime time)
        {
            return time.ToUniversalTime().ToString(@"yyyy-MM-dd\Thh:mm:ss", CultureInfo.InvariantCulture);
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

        private void SaveToExcel(List<FacebookPostComment> entries)
        {
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            string fileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Social Media Assessment.xlsx");

            string CnStr = ("Provider=Microsoft.ACE.OLEDB.12.0;" + ("Data Source=" + (fileName + (";" + "Extended Properties=\"Excel 12.0 Xml;HDR=NO;\""))));
            OleDbConnection oledbConn = new OleDbConnection(CnStr);
            var existingFile = new FileInfo(fileName);
            try
            { // Write a certain number of entries, close, and open again to write to prevent excessive amount of data.
                using (var package = new ExcelPackage(existingFile))
                {
                    // Get the work book in the file
                    var workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            // Get the first row
                            var currentWorksheet = workBook.Worksheets["7 Facebook raw data"];
                            currentWorksheet.Cells[2, 1].Value = _socialMediaStat.TotalDays;
                            currentWorksheet.Cells[2, 2].Value = _totalFans;
                            currentWorksheet.Cells[2, 3].Value = _totalPosts;
                            currentWorksheet.Cells[2, 4].Value = _totalLikes;
                            currentWorksheet.Cells[2, 5].Value = _totalComments;
                            currentWorksheet.Cells[2, 6].Value = _totalShares;
                            package.Save();
                        }
                    }
                }
                int entryPostion = 0; 
                int rowPosition = 4;
                while (entryPostion < entries.Count)
                {
                    WriteFBCommentsPartial(entryPostion, rowPosition, entries, fileName);
                    entryPostion += _rowPerTime;
                    rowPosition += _rowPerTime;
                }
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        private void WriteFBCommentsPartial(int entryNextPosition, int rowPostion, List<FacebookPostComment> entries, String fileName)
        {
            var existingFile = new FileInfo(fileName); 
            using (var package = new ExcelPackage(existingFile))
            {
            // Get the work book in the file
                var workBook = package.Workbook;
                if (workBook != null)
                {
                    if (workBook.Worksheets.Count > 0)
                    {
                        var currentWorksheet = workBook.Worksheets["7 Facebook raw data"];
                        for (int i = 0;i<_rowPerTime;i++)
                        {
                            int index = i+entryNextPosition;
                            if (index < entries.Count)
                            {
                                FacebookPostComment entry = entries[index];
                                currentWorksheet.Cells[rowPostion, 1].Value = entry.PostUrl;
                                currentWorksheet.Cells[rowPostion, 2].Value = entry.Post;
                                currentWorksheet.Cells[rowPostion, 3].Value = ConvertToJson(DateTime.Parse(entry.PostTimestamp));
                                if (entry.PostComment != null)

                                    currentWorksheet.Cells[rowPostion, 4].Value = entry.PostComment;
                                if (entry.ResponseTime != null)
                                {

                                    currentWorksheet.Cells[rowPostion, 5].Value = entry.Response;
                                    currentWorksheet.Cells[rowPostion, 6].Value = ConvertToJson(DateTime.Parse(entry.ResponseTime));
                                }
                            }
                            else
                            { break; }
                            rowPostion++;
                        }
                        package.Save();
                    }
                }
            }        
        }
        private void SaveToDB(List<FacebookPost> posts)
        {
            Database context = new Database();
            // clear all existing postComment of this assessment
            context.facebookPostComments.RemoveRange(context.facebookPostComments.Where(x => x.AssessmentId == _assessment.Id));
            foreach (FacebookPost post in posts)
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
                        facebookPostComment.PostTimestamp = comment.created_time;
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
        public string id { get; set; }
        public string message { get; set; }
        public FacebookComments comments { get; set; }
        public FacebookLikes likes { get; set; }
        public Share shares { get; set; }
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
        public string next { get; set; }
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
        public int likes { get; set; }
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
