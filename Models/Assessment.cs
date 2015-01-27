using System ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MCIFramework.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public string Organisation { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Boolean IsStrategy { get; set; }
        public Boolean IsSocialMedia { get; set; }
        public Boolean IsFacebook { get; set; }
        public Boolean IsTwitter { get; set; }
        public Boolean IsYoutube { get; set; }
        public Boolean IsWeb { get; set; }
        public string FacebookUsername { get; set; }
        public string TwitterUsername { get; set; }
        public string YoutubeId { get; set; }
        public string WebUrl { get; set; }
        public string TopPage1 { get; set; }
        public string TopPageUrl1 { get; set; }
        public string TopPage2 { get; set; }
        public string TopPageUrl2 { get; set; }
        public string TopPage3 { get; set; }
        public string TopPageUrl3 { get; set; }
        public string TopPage4 { get; set; }
        public string TopPageUrl4 { get; set; }
        public string TopPage5 { get; set; }
        public string TopPageUrl5 { get; set; }
        public string Audience1 { get; set; }
        public string Audience2 { get; set; }
        public string Audience3 { get; set; }
        public string Audience1Scenario1 { get; set; }
        public string Audience1Scenario2 { get; set; }
        public string Audience1Scenario3 { get; set; }
        public string Audience2Scenario1 { get; set; }
        public string Audience2Scenario2 { get; set; }
        public string Audience2Scenario3 { get; set; }
        public string Audience3Scenario1 { get; set; }
        public string Audience3Scenario2 { get; set; }
        public string Audience3Scenario3 { get; set; }
        public string Audience1Keyword1 { get; set; }
        public string Audience1Keyword2 { get; set; }
        public string Audience1Keyword3 { get; set; }
        public string Audience2Keyword1 { get; set; }
        public string Audience2Keyword2 { get; set; }
        public string Audience2Keyword3 { get; set; }
        public string Audience3Keyword1 { get; set; }
        public string Audience3Keyword2 { get; set; }
        public string Audience3Keyword3 { get; set; }
        public DateTime? ReportGenerationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


    }

    public class dataUploaded
    {
        public int level { get; set; }
        public string title { get; set; }
        public string score { get; set; }
        public string description { get; set; }
        public string criteria { get; set; }
        public string indicator { get; set; }
        public string recommendations { get; set; }
        public string assessor { get; set; }
    }

    //jason
    public class DetailsLevel2
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string criteria { get; set; }
        public string recommendations { get; set; }
        public string score { get; set; }
        public string indicator_for { get; set; }
        public string assessor { get; set; }
    }

    public class DetailsLevel1
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string criteria { get; set; }
        public string recommendations { get; set; }
        public string score { get; set; }
        public List<DetailsLevel2> details_level_2 { get; set; }
        //public List<object> details_level_2 { get; set; }
    }

    public class Result
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string score { get; set; }
        public List<DetailsLevel1> details_level_1 { get; set; }
    }

    public class RootObject
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<Result> results { get; set; }
    }





}
