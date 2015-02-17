using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using System.Text.RegularExpressions;
using LinqToTwitter;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Globalization;
using System.Reflection;
using OfficeOpenXml;

namespace MCIFramework.Helper
{
    public class TwitterImporter
    {
        private int _rowPerTime = Int32.Parse(Properties.Settings.Default.PartialExcelRowWriting);
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private string _twitterUsername;
        private TwitterContext _twitterContext;
        private int StatusQueryCount = Int32.Parse(Properties.Resources._api_twitter_status_query_max_count);
        private int SearchQueryCount = Int32.Parse(Properties.Resources._api_twitter_search_query_max_count);

        private int _twitterFollowers = 0;
        private int _twitterTweets = 0;
        private int _twitterFavourites = 0;
        private int _twitterRetweets = 0;
        private int _twitterReplies = 0;
        private string _loggedInName;
        public TwitterImporter(Assessment assessment, SocialMediaStat socialMediaStat, String loggedInName)
        {
            try
            {
                _assessment = assessment;
                _socialMediaStat = socialMediaStat;
                string name = _assessment.FacebookUsername.ToLower();
                if (name.Contains("https") || name.Contains("twitter.com") || name.Contains("http"))
                {
                    String[] splits = name.Split('/');
                    _twitterUsername = splits[splits.Length - 1];
                }
               
                _loggedInName = loggedInName;
                Database db = new Database();
                API token = db.apis.Where(x => x.Name == "TwitterToken").FirstOrDefault();
                API tokenSecret = db.apis.Where(x => x.Name == "TwitterTokenSecret").FirstOrDefault();
                var authorizer = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = Properties.Resources._api_twitter_consumer_key,
                        ConsumerSecret = Properties.Resources._api_twitter_consumer_secret,
                        OAuthToken = token.Value,
                        OAuthTokenSecret = tokenSecret.Value
                    }
                };

                _twitterContext = new TwitterContext(authorizer);
            }
            catch (Exception e)
            {

            }
        }

        public void Process()
        {
            try
            {
                List<Status> mentionTweets = GetMentionedTweet();
                List<Status> filteredMentionTweets = mentionTweets.Where(x => x.CreatedAt <= _assessment.EndDate && x.CreatedAt >= _assessment.StartDate).ToList();

                List<Status> userTweets = GetAllTweetsOfuser(_loggedInName);
                List<Status> filteredUserTweets = userTweets.Where(x => x.CreatedAt <= _assessment.EndDate && x.CreatedAt >= _assessment.StartDate).ToList();

                GetTwitterUserInfo(filteredUserTweets);
                GetTotalRepliesToTweetsFromUser(filteredUserTweets, mentionTweets);
                List<TwitterTweet> excelEntries = GetMentionedTweetsAndReplies(filteredMentionTweets, filteredUserTweets);
                SaveToDB(excelEntries);
                SaveToExcelPackage(excelEntries);

            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }

        }

        private List<Status> GetAllTweetsOfuser(string twitterUserName)
        {
            List<Status> result = new List<Status>();

            var userStatusResponse =
                      from tweet in _twitterContext.Status
                      where tweet.Type == StatusType.User &&
                      tweet.Count == StatusQueryCount &&
                      tweet.ScreenName == twitterUserName
                      select tweet;

            result.AddRange(userStatusResponse.ToList());
            if (result.Count != 0)
            {
                ulong maxID = userStatusResponse.ToList().Min(status => status.StatusID) - 1;
                ulong sinceId = result[result.Count - 1].StatusID;
                do
                {

                    userStatusResponse =
                       (from tweet in _twitterContext.Status
                        where tweet.Type == StatusType.User &&
                              tweet.ScreenName == twitterUserName &&
                              tweet.Count == StatusQueryCount &&
                              tweet.MaxID == maxID
                        select tweet);


                    if (userStatusResponse.ToList().Count > 0)
                    {
                        // first tweet processed on current query
                        maxID = userStatusResponse.ToList().Min(status => status.StatusID) - 1;

                        result.AddRange(userStatusResponse.ToList());
                    }
                    if (result[result.Count - 1].CreatedAt < _assessment.StartDate) // stop if tweets are created before assessment start datae
                        break;
                }
                while (userStatusResponse.ToList().Count != 0);
            }
            return result;
        }

        private List<Status> GetAll7DaysTweetsMentioningUser(string twitterUserName)
        {
            List<Status> result = new List<Status>();

            List<Search> userStatusResponse =
                      (from tweet in _twitterContext.Search
                       where tweet.Query == "@" + twitterUserName &&
                       tweet.Type == SearchType.Search &&
                       tweet.ResultType == ResultType.Recent &&
                       tweet.Count == SearchQueryCount
                       select tweet).ToList();


            foreach (Search search in userStatusResponse)
            {
                if (search.Statuses != null)
                    result.AddRange(search.Statuses);
            }
            ulong maxID = result.Min(status => status.StatusID) - 1;
            do
            {

                userStatusResponse =
                   (from tweet in _twitterContext.Search
                    where tweet.Query == "@" + twitterUserName &&
                          tweet.Type == SearchType.Search &&
                          tweet.ResultType == ResultType.Recent &&
                          tweet.Count == SearchQueryCount &&
                          tweet.MaxID == maxID
                    select tweet).ToList();


                foreach (Search search in userStatusResponse)
                {
                    if (search.Statuses != null && search.Statuses.Count > 0)
                    {
                        result.AddRange(search.Statuses);
                        maxID = search.Statuses.Min(status => status.StatusID) - 1;
                    }
                    result.AddRange(search.Statuses.ToList());
                }
                if (userStatusResponse.Count > 0)
                    if (result.Min(x => x.CreatedAt) < _assessment.StartDate) // stop if tweets are created before assessment start datae
                        break;
            }
            while (userStatusResponse[0].Statuses.Count != 0);
            return result;
        }

        private List<Status> GetMentionedTweet()
        {
            List<Status> result = new List<Status>();

            var userStatusResponse =
                      from tweet in _twitterContext.Status
                      where tweet.Type == StatusType.Mentions &&
                      tweet.Count == StatusQueryCount

                      select tweet;

            result.AddRange(userStatusResponse.ToList());
            if (result.Count != 0)
            {
                ulong maxID = userStatusResponse.ToList().Min(status => status.StatusID) - 1;
                ulong sinceId = result[result.Count - 1].StatusID;
                do
                {

                    userStatusResponse =
                       (from tweet in _twitterContext.Status
                        where tweet.Type == StatusType.Mentions &&
                              tweet.Count == StatusQueryCount &&
                              tweet.MaxID == maxID
                        select tweet);


                    if (userStatusResponse.ToList().Count > 0)
                    {
                        // first tweet processed on current query
                        maxID = userStatusResponse.ToList().Min(status => status.StatusID) - 1;

                        result.AddRange(userStatusResponse.ToList());
                    }
                    if (result[result.Count - 1].CreatedAt < _assessment.StartDate) // stop if tweets are created before assessment start datae
                        break;
                }
                while (userStatusResponse.ToList().Count != 0);
            }
            return result;
        }

        private void GetTwitterUserInfo(List<Status> tweets)
        {
            _twitterTweets += tweets.Count;
            foreach (Status tweet in tweets)
            {
                if (tweet.FavoriteCount != null && tweet.RetweetedStatus.StatusID == 0)// if this is not a retweet
                    _twitterFavourites += (int)tweet.FavoriteCount;


                if (tweet.RetweetCount != null && tweet.RetweetedStatus.StatusID == 0)// if this is not a retweet
                    _twitterRetweets += tweet.RetweetCount;
            }
            var userInfo = from user in _twitterContext.User
                           where user.Type == UserType.Lookup &&
                           user.ScreenNameList == _loggedInName
                           select user;
            foreach (var user in userInfo)
            {
                _twitterFollowers = user.FollowersCount;
            }
        }

        private void GetTotalRepliesToTweetsFromUser(List<Status> userTweets, List<Status> mentionTweets)
        {
            foreach (Status userTweet in userTweets)
            {
                foreach (Status mentionTweet in mentionTweets)
                {
                    if (mentionTweet.InReplyToStatusID == userTweet.StatusID)
                        _twitterReplies++;
                }
            }
        }

        private List<TwitterTweet> GetMentionedTweetsAndReplies(List<Status> mentionTweets, List<Status> filterTweets)
        {
            List<TwitterTweet> result = new List<TwitterTweet>();
            foreach (Status mentionTweet in mentionTweets)
            {
                TwitterTweet entry = new TwitterTweet();
                entry.AssessmentId = _assessment.Id;
                entry.Tweet = mentionTweet.Text;
                entry.TweetTimestamp = mentionTweet.CreatedAt;
                entry.TweetUrl = "https://twitter.com/" + mentionTweet.User.ScreenNameResponse + "/status/" + mentionTweet.StatusID.ToString();
                entry.TwitterId = mentionTweet.User.ScreenNameResponse;
                var replyTweets = filterTweets.Where(x => x.InReplyToStatusID == mentionTweet.StatusID).ToList();
                if (replyTweets.Count != 0)
                {
                    Status firstReplyTweet = replyTweets.OrderBy(x => x.CreatedAt).ToList()[0];
                    entry.Response = firstReplyTweet.Text;
                    entry.ResponseTimestamp = firstReplyTweet.CreatedAt;
                    entry.ResponseUrl = "https://twitter.com/" + firstReplyTweet.User.ScreenNameResponse + "/status/" + firstReplyTweet.StatusID.ToString(); ;
                }
                result.Add(entry);
            }

            // Remove duplicate if there is
            List<TwitterTweet> distinctItems = result.GroupBy(x => x.TweetUrl).Select(y => y.First()).ToList();

            return distinctItems;
        }

        private void SaveToDB(List<TwitterTweet> entries)
        {
            Database context = new Database();
            // clear all existing postComment of this assessment
            context.twitterTweets.RemoveRange(context.twitterTweets.Where(x => x.AssessmentId == _assessment.Id));
            context.twitterTweets.AddRange(entries);
            context.SaveChanges();
        }

        private void SaveToExcel(List<TwitterTweet> entries)
        {
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            string fileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _assessment.Organisation + " " + _assessment.Title + " Social Media Assessment.xlsx");

            string CnStr = ("Provider=Microsoft.ACE.OLEDB.12.0;" + ("Data Source=" + (fileName + (";" + "Extended Properties=\"Excel 12.0 Xml;HDR=NO;\""))));
            OleDbConnection oledbConn = new OleDbConnection(CnStr);
            try
            {
                oledbConn.Open();
                OleDbCommand command = oledbConn.CreateCommand();
                string strSQL;


                strSQL = "INSERT INTO [8 Twitter raw data$A1:F1] VALUES ( @1, @2, @3,  @4,  @5, @6)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _socialMediaStat.TotalDays);
                command.Parameters.AddWithValue("@2", _twitterFollowers);
                command.Parameters.AddWithValue("@3", _twitterTweets);
                command.Parameters.AddWithValue("@4", _twitterFavourites);
                command.Parameters.AddWithValue("@5", _twitterReplies);
                command.Parameters.AddWithValue("@6", _twitterRetweets);
                command.CommandType = CommandType.Text;
                command.CommandText = strSQL;
                OleDbTransaction myTransaction = oledbConn.BeginTransaction();
                command.Transaction = myTransaction;
                command.ExecuteNonQuery();

                int startTweetRow = 3;
                foreach (TwitterTweet entry in entries)
                {
                    if (entry.Response != null)
                    {
                        command.CommandText = "INSERT INTO [8 Twitter raw data$A" + startTweetRow + ":F" + startTweetRow +
                            "] VALUES (@1, @2, @3, @4, @5, @6)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@1", entry.Tweet);
                        command.Parameters.AddWithValue("@2", entry.TweetUrl);
                        command.Parameters.AddWithValue("@3", ConvertToJson((DateTime)entry.TweetTimestamp));
                        command.Parameters.AddWithValue("@4", entry.Response);
                        command.Parameters.AddWithValue("@5", entry.ResponseUrl);
                        command.Parameters.AddWithValue("@6", ConvertToJson((DateTime)entry.ResponseTimestamp));
                    }
                    else
                    {
                        command.CommandText = "INSERT INTO [8 Twitter raw data$A" + startTweetRow + ":C" + startTweetRow +
    "] VALUES (@1, @2, @3)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@1", entry.Tweet);
                        command.Parameters.AddWithValue("@2", entry.TweetUrl);
                        command.Parameters.AddWithValue("@3", ConvertToJson((DateTime)entry.TweetTimestamp));

                    }
                    command.ExecuteNonQuery();
                    startTweetRow++;
                }


                myTransaction.Commit();
                oledbConn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SaveToExcelPackage(List<TwitterTweet> entries)
        {
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            string fileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _assessment.Organisation + " " + _assessment.Title + " Social Media Assessment.xlsx");
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
                            var currentWorksheet = workBook.Worksheets["8 Twitter raw data"];
                            currentWorksheet.Cells[2, 1].Value = _socialMediaStat.TotalDays;
                            currentWorksheet.Cells[2, 2].Value = _twitterFollowers;
                            currentWorksheet.Cells[2, 3].Value = _twitterTweets;
                            currentWorksheet.Cells[2, 4].Value = _twitterFavourites;
                            currentWorksheet.Cells[2, 5].Value = _twitterReplies;
                            currentWorksheet.Cells[2, 6].Value = _twitterRetweets;
                            package.Save();
                        }
                    }
                }
                int entryPostion = 0;
                int rowPosition = 4;
                while (entryPostion < entries.Count)
                {
                    WriteTwitterPartial(entryPostion, rowPosition, entries, fileName);
                    entryPostion += _rowPerTime;
                    rowPosition += _rowPerTime;
                }
            }

            catch (Exception e)
            {
                throw e;
            }



        }

        private void WriteTwitterPartial(int entryNextPosition, int rowPostion, List<TwitterTweet> entries, String fileName)
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
                        var currentWorksheet = workBook.Worksheets["8 Twitter raw data"];
                        for (int i = 0; i < _rowPerTime; i++)
                        {
                            int index = i + entryNextPosition;
                            if (index < entries.Count)
                            {
                                TwitterTweet entry = entries[index];
                                currentWorksheet.Cells[rowPostion, 1].Value = entry.Tweet;
                                currentWorksheet.Cells[rowPostion, 2].Value = entry.TweetUrl;
                                if (entry.TweetTimestamp != null)
                                    currentWorksheet.Cells[rowPostion, 3].Value = ConvertToJson((DateTime)entry.TweetTimestamp);
                                if (entry.Response != null)
                                {
                                    currentWorksheet.Cells[rowPostion, 4].Value = entry.Response;
                                    currentWorksheet.Cells[rowPostion, 5].Value = entry.ResponseUrl;
                                    if(entry.ResponseTimestamp!=null)
                                        currentWorksheet.Cells[rowPostion, 6].Value = ConvertToJson((DateTime)entry.ResponseTimestamp);
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
                            var currentWorksheet = workBook.Worksheets["3 Twitter responding"];
                            for (int i = 0; i < entries; i++)
                            {
                                int row = i + startRow;
                                currentWorksheet.Cells[row, 1].FormulaR1C1 = currentWorksheet.Cells[3, 1].FormulaR1C1;
                                currentWorksheet.Cells[row, 3].FormulaR1C1 = currentWorksheet.Cells[3, 3].FormulaR1C1;

                                currentWorksheet.Cells[row, 4].FormulaR1C1 = currentWorksheet.Cells[3, 4].FormulaR1C1;
                                currentWorksheet.Cells[row, 7].FormulaR1C1 = currentWorksheet.Cells[3, 7].FormulaR1C1;
                                currentWorksheet.Cells[row, 8].FormulaR1C1 = currentWorksheet.Cells[3, 8].FormulaR1C1;
                                currentWorksheet.Cells[row, 9].FormulaR1C1 = currentWorksheet.Cells[3, 9].FormulaR1C1;
                                
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

        public SocialMediaStat GetDataToStore()
        {
            _socialMediaStat.TwitterFavourites = _twitterFavourites;
            _socialMediaStat.TwitterFollowers = _twitterFollowers;
            _socialMediaStat.TwitterReplies = _twitterReplies;
            _socialMediaStat.TwitterRetweets = _twitterRetweets;
            _socialMediaStat.TwitterTweets = _twitterTweets;

            return _socialMediaStat;
        }

        private String ConvertToJson(DateTime time)
        {
            return time.ToString(@"ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
        }
    }


    /// <summary>
    /// Extends the LinqToTwitter Library
    /// </summary>

}
