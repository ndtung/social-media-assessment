using System;
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
        private int _rowPerTime = Int32.Parse(Properties.Settings.Default.PartialExcelRowWriting);
        private int _maxComment = Int32.Parse(Properties.Settings.Default.FbMaxComments);
        private string _commentsPerRequest = Properties.Settings.Default.GraphAPICommentPerRequest;
        private string _postsPerRequest = Properties.Settings.Default.GraphAPIPostPerRequest;
        private string _albumPerRequest = Properties.Settings.Default.GraphAPIAlbumPerRequest;
        private int _totalLikes = 0;
        private int _totalComments = 0;
        private int _totalPosts = 0;
        private int _totalShares = 0;
        private int _totalFans = 0;
        private string _pageId;
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private FacebookClient _client;
        private string _facebookNameToProcess;

        private int _maximumPostComment = Int32.Parse(Properties.Resources._api_facebook_maximum_comments_to_retrieve_of_a_post);
        public FacebookImporter(Assessment assessment, SocialMediaStat socialMediaStat)
        {
            try
            {
                _assessment = assessment;
                _socialMediaStat = socialMediaStat;
                string name = _assessment.FacebookUsername.ToLower();
                if (name.Contains("https") || name.Contains("facebook.com") || name.Contains("http"))
                {
                    String[] splits = name.Split('/');
                    _facebookNameToProcess = splits[splits.Length - 1];
                }
                else
                    _facebookNameToProcess = _assessment.FacebookUsername;
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Process()
        {
            try
            {
                GetPageId();
                List<FacebookAlbum> actualAlbumPosts = GetAlbumCreatedInPeriod();
                List<FacebookPost> actualPosts = GetFacebookPosts();
                GetAllComments(actualPosts);
                GetAllAlbumComments(actualAlbumPosts);
                GetFirstReplyFromPage(actualPosts);
                GetFirstAlbumReplyFromPage(actualAlbumPosts);
                SaveToDB(actualPosts,actualAlbumPosts);
                Database db = new Database();
                SaveToExcel(db.facebookPostComments.Where(x => x.AssessmentId == _assessment.Id).OrderByDescending(x=>x.PostTimestamp).ToList());
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }


        private void GetPageId()
        {
            var info = _client.Get(_facebookNameToProcess + "/?fields=id,likes").ToString();
            FacebookPage pageInfo = JsonConvert.DeserializeObject<FacebookPage>(info);
            _pageId = pageInfo.id;
            _totalFans = pageInfo.likes;
        }

        private List<FacebookAlbum> GetAlbumCreatedInPeriod()
        {
            List<FacebookAlbum> pageAlbums = new List<FacebookAlbum>();
            // Getting all page posts after assessment start date
            var albums = _client.Get(_facebookNameToProcess + "/albums?fields=id,likes.summary(true),comments.summary(true).limit(" + _commentsPerRequest + "),sharedposts,name,link&limit=" + _albumPerRequest).ToString();
            FacebookAlbums result = JsonConvert.DeserializeObject<FacebookAlbums>(albums);
            if (result.data!=null)
                pageAlbums.AddRange(result.data);

            if (result.paging != null && result.paging.next != null)
            {
                FacebookAlbums pageResult;
                string nextRequest = result.paging.next;
                do
                {
                    var pageStatus = _client.Get(nextRequest).ToString();
                    pageResult = JsonConvert.DeserializeObject<FacebookAlbums>(pageStatus);
                    if (pageResult.data != null)
                        pageAlbums.AddRange(pageResult.data);
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
            // Filter albums
            List<FacebookAlbum> actualPosts = pageAlbums.Where(p => p.name != null && ConvertToDateTime(p.created_time) <= _assessment.EndDate && ConvertToDateTime(p.created_time) >= _assessment.StartDate).ToList();
            return actualPosts;
        }

        private List<FacebookPost> GetFacebookPosts()
        {
            List<FacebookPost> pagePosts = new List<FacebookPost>();
            // Getting all page posts after assessment start date
            var statuses = _client.Get(_facebookNameToProcess + "/posts?fields=id,actions,message,shares,comments.summary(true).limit("+_commentsPerRequest+"),likes.summary(true)&limit="+_postsPerRequest).ToString();
            FacebookPosts result = JsonConvert.DeserializeObject<FacebookPosts>(statuses);
            if (result.data!=null)
                pagePosts.AddRange(result.data);

            if (result.paging != null && result.paging.next != null)
            {
                FacebookPosts pageResult;
                string nextRequest = result.paging.next;
                do
                {
                    var pageStatus = _client.Get(nextRequest).ToString();
                    pageResult = JsonConvert.DeserializeObject<FacebookPosts>(pageStatus);
                    if (pageResult.data!=null)
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
                _totalPosts += posts.Count;
            else
                _totalPosts += 0;
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
                        while (true && post.comments.data.Count < _maxComment);// Get all maximum comments
                    }
                    if (post.shares != null)
                        _totalShares += post.shares.count;
                    if (post.likes != null)
                        _totalLikes += post.likes.summary.total_count;
                    if (post.comments != null)
                        _totalComments += post.comments.summary.total_count;
                    post.comments.data.OrderBy(x => x.created_time);
                }
            }

        }

        private void GetAllAlbumComments(List<FacebookAlbum> albums)
        {
            if (albums.Count != 0)
                _totalPosts += albums.Count;
            else
                _totalPosts += 0;
            foreach (FacebookAlbum post in albums)
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
                        while (true && post.comments.data.Count < _maxComment);// Get all maximum comments
                    }
                    if (post.sharedposts != null && post.sharedposts.data!=null)
                        _totalShares += post.sharedposts.data.Count;
                    if (post.likes != null)
                        _totalLikes += post.likes.summary.total_count;
                    if (post.comments != null)
                        _totalComments += post.comments.summary.total_count;
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

        private void GetFirstAlbumReplyFromPage(List<FacebookAlbum> actualAlbumPosts)
        {
            foreach (FacebookAlbum post in actualAlbumPosts)
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
            string fileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx",_assessment.Organisation+ " "+_assessment.Title+ " Social Media Assessment.xlsx");

            var existingFile = new FileInfo(fileName);
            WriteFormulaCells(4, entries.Count, fileName);
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

        private void WriteFormulaCells(int startRow, int entries, String fileName)
        {
            try
            {
                var existingFile = new FileInfo(fileName);
                using (var package = new ExcelPackage(existingFile))
                {
                    var workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            // Get the first row
                            var currentWorksheet = workBook.Worksheets["1 Facebook responding"];
                            for (int i = 0; i < entries; i++)
                            {
                                int row = i + startRow;
                                currentWorksheet.Cells[row, 1].FormulaR1C1 = currentWorksheet.Cells[3, 1].FormulaR1C1;
                                currentWorksheet.Cells[row, 2].FormulaR1C1 = currentWorksheet.Cells[3, 2].FormulaR1C1;
                                currentWorksheet.Cells[row, 4].FormulaR1C1 = currentWorksheet.Cells[3, 4].FormulaR1C1;
                                currentWorksheet.Cells[row, 5].FormulaR1C1 = currentWorksheet.Cells[3, 5].FormulaR1C1;
                                currentWorksheet.Cells[row, 8].FormulaR1C1 = currentWorksheet.Cells[3, 8].FormulaR1C1;
                                currentWorksheet.Cells[row, 9].FormulaR1C1 = currentWorksheet.Cells[3, 9].FormulaR1C1;
                                currentWorksheet.Cells[row, 10].FormulaR1C1 = currentWorksheet.Cells[3, 10].FormulaR1C1;
                            }
                            package.Save();
                        }
                    }
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

        private void SaveToDB(List<FacebookPost> posts,List<FacebookAlbum> albums )
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

            foreach (FacebookAlbum album in albums)
            {
                if (album.comments != null)
                {
                    foreach (FacebookComment comment in album.comments.data)
                    {
                        FacebookPostComment facebookPostComment = new FacebookPostComment();
                        facebookPostComment.AssessmentId = _assessment.Id;
                        facebookPostComment.FacebookId = _pageId;
                        facebookPostComment.Post = album.name;
                        facebookPostComment.PostComment = comment.message;
                        facebookPostComment.PostTimestamp = comment.created_time;
                        facebookPostComment.PostUrl = album.link;
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
                    facebookPostComment.Post = album.name;
                    facebookPostComment.PostTimestamp = album.created_time;
                    facebookPostComment.PostUrl = album.link;
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
        public int total_count { get; set; }
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

    public class FacebookAlbums
    {
        public List<FacebookAlbum> data { get; set; }
        public Paging paging { get; set; }
    }

    public class FacebookAlbum
    {
        public string id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public FacebookComments comments { get; set; }
        public FacebookLikes likes { get; set; }
        public SharedPosts sharedposts { get; set; }
        public string created_time { get; set; }
        
    }
    public class SharedPosts
    {
        public List<FacebookPost> data { get; set; }
        public Paging paging { get; set; }
    }
}
