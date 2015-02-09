using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json.Linq;
using MCIFramework.Models;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Reflection;

namespace MCIFramework.Helper
{
    public class YoutubeImporter
    {
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat;
        private String _channelId;
        private ulong _totalLikes = 0;
        private ulong _totalDislikes = 0;
        private ulong _totalViews = 0;
        private ulong _subscribers = 0;
        private int _totalVideos = 0;
        private YouTubeService _youtube;
        

        public YoutubeImporter(Assessment assessment, SocialMediaStat socialMediaStat)
        {
            _assessment = assessment;
            _socialMediaStat = socialMediaStat;
            _channelId = assessment.YoutubeId;
            _youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = "rrmciframework",
                ApiKey = Properties.Resources._api_youtube_api_key,
            });

        }

        public void Process()
        {
            try
            {
                ChannelsResource.ListRequest channelListRequest = _youtube.Channels.List("id");

                // Test if user put user name instead of channel id
                channelListRequest.ForUsername = _channelId;
                ChannelListResponse channelsListResponse = channelListRequest.Execute();

                string channelId = "0";
                if (channelsListResponse.Items.Count != 0)
                    channelId = channelsListResponse.Items[0].Id;

                //User put correct channel ID
                SearchResource.ListRequest listRequest = _youtube.Search.List("snippet");
                if (channelId == "0") // cant find channel id from user name
                    listRequest.ChannelId = _channelId;//
                else // cand find channel id from user name
                    listRequest.ChannelId = channelId;

                listRequest.MaxResults = 50;
                listRequest.Type = "video";
                listRequest.PublishedAfter = _assessment.StartDate;
                listRequest.PublishedBefore = _assessment.EndDate;


                // Get all uploaded videos and store to uploadedVideos
                SearchListResponse resp = listRequest.Execute();
                IList<SearchResult> uploadedVideos = resp.Items;
                string nextPageToken = resp.NextPageToken;
                while (nextPageToken != null)
                {
                    listRequest.PageToken = nextPageToken;
                    SearchListResponse respPage = listRequest.Execute();
                    var resultsPage = respPage.Items;
                    foreach (SearchResult i in resultsPage)
                        uploadedVideos.Add(i);
                    nextPageToken = respPage.NextPageToken;
                    if (uploadedVideos.Count== Int32.Parse(Properties.Resources._api_youtube_maximum_videos))// Prevent excessive use of API calls
                        break;
                }


                // Aggregate data
                foreach (SearchResult video in uploadedVideos)
                {
                    // video.Id
                    VideosResource.ListRequest vidReq = _youtube.Videos.List("statistics");
                    vidReq.Id = video.Id.VideoId;
                    VideoListResponse vidResp = vidReq.Execute();
                    Video item;
                    if (vidResp.Items.Count != 0)
                    {
                        item = vidResp.Items[0];
                        if (item.Statistics.LikeCount != null)
                            _totalLikes += (ulong)item.Statistics.LikeCount;
                        if (item.Statistics.DislikeCount != null)
                            _totalDislikes += (ulong)item.Statistics.DislikeCount;
                        if (item.Statistics.ViewCount != null)
                            _totalViews += (ulong)item.Statistics.ViewCount;
                    }
                }
                _totalVideos += uploadedVideos.Count;
                // Grab number of subscribers
                ChannelsResource.ListRequest channelReq = _youtube.Channels.List("statistics");
                channelReq.Id = channelId;
                ChannelListResponse channelResp = channelReq.Execute();
                if (channelResp.Items.Count != 0)
                {
                    if (channelResp.Items[0].Statistics.SubscriberCount != null)
                        _subscribers += (ulong)channelResp.Items[0].Statistics.SubscriberCount;

                }

                // Save to Excel
                SaveToExcel();
               
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }

        }


        private void SaveToExcel()
        {
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            string fileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Social Media Assessment.xlsx");

            string CnStr = ("Provider=Microsoft.ACE.OLEDB.12.0;" + ("Data Source=" + (fileName + (";" + "Extended Properties=\"Excel 12.0 Xml;HDR=NO;\""))));
            OleDbConnection oledbConn = new OleDbConnection(CnStr);
            try
            {
                oledbConn.Open();
                OleDbCommand command = oledbConn.CreateCommand();
                string strSQL;

                //Load the insert statement into the string variable, with all of the passed info
                strSQL = "INSERT INTO [9 YouTube raw data$A1:F1] VALUES (" + _socialMediaStat.TotalDays + ", " + _subscribers + ", " + _totalVideos + ", "
                    + _totalViews + "," + _totalLikes + "," + _totalDislikes+")";

                OleDbTransaction myTransaction = oledbConn.BeginTransaction();
                command.Transaction = myTransaction;
                command.CommandText = strSQL;
                command.ExecuteNonQuery();

                myTransaction.Commit();
                oledbConn.Close();
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
        }

        public SocialMediaStat GetDataToStore()
        {
            _socialMediaStat.YoutubeSubscribers = _subscribers.ToString();
            _socialMediaStat.YoutubeVideoDislikes = _totalDislikes.ToString();
            _socialMediaStat.YoutubeVideoLikes = _totalLikes.ToString();
            _socialMediaStat.YoutubeVideos = _totalVideos.ToString();
            _socialMediaStat.YoutubeVideoViews = _totalViews.ToString();
            return _socialMediaStat;
        }
    }
}
