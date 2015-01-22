using System ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Models
{
    public class Assessment
    {
        public int id { get; set; }
        public string Organisation { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Boolean IsStrategy { get; set; }
        public Boolean IsFacebook { get; set; }
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
        public DateTime ReportGenerationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
