using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace MCIFramework.Models
{
    public class Assessment : IDataErrorInfo
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

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string propertyName]
        {
            get
            {
                string validationResult = null;
                switch (propertyName)
                {
                    case "IsSocialMedia":
                        validationResult = String.Empty;
                        break;
                    case "Organisation":
                        validationResult = ValidateOrganisation();
                        break;
                    case "Title":
                        validationResult = ValidateTitle();
                        break;
                    case "FacebookUsername":
                        validationResult = ValidateFacebookUserName();
                        break;
                    case "TwitterUsername":
                        validationResult = ValidateTwitterUserName();
                        break;
                    case "YoutubeId":
                        validationResult = ValidateYoutubeId();
                        break;
                    case "StartDate":
                        validationResult = ValidateDateTime(this.StartDate);
                        break;
                    case "EndDate":
                        validationResult = ValidateDateTime(this.EndDate);
                        break;
                    case "WebUrl":
                        validationResult = ValidateUrl(this.WebUrl);
                        break;
                    case "TopPage1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.TopPage1);
                        break;
                    case "TopPage2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.TopPage2);
                        break;
                    case "TopPage3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.TopPage3);
                        break;
                    case "TopPage4":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.TopPage4);
                        break;
                    case "TopPage5":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.TopPage5);
                        break;
                    case "TopPageUrl1":
                        validationResult = ValidateUrl(this.TopPageUrl1);
                        break;
                    case "TopPageUrl2":
                        validationResult = ValidateUrl(this.TopPageUrl2);
                        break;
                    case "TopPageUrl3":
                        validationResult = ValidateUrl(this.TopPageUrl3);
                        break;
                    case "TopPageUrl4":
                        validationResult = ValidateUrl(this.TopPageUrl3);
                        break;
                    case "TopPageUrl5":
                        validationResult = ValidateUrl(this.TopPageUrl5);
                        break;
                    case "Audience1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1);
                        break;
                    case "Audience2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2);
                        break;
                    case "Audience3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3);
                        break;
                    case "Audience1Keyword1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Keyword1);
                        break;
                    case "Audience1Keyword2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Keyword2);
                        break;
                    case "Audience1Keyword3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Keyword3);
                        break;
                    case "Audience2Keyword1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Keyword1);
                        break;
                    case "Audience2Keyword2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Keyword2);
                        break;
                    case "Audience2Keyword3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Keyword3);
                        break;
                    case "Audience3Keyword1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Keyword1);
                        break;
                    case "Audience3Keyword2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Keyword2);
                        break;
                    case "Audience3Keyword3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Keyword3);
                        break;
                    case "Audience1Scenario1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Scenario1);
                        break;
                    case "Audience1Scenario2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Scenario2);
                        break;
                    case "Audience1Scenario3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience1Scenario3);
                        break;
                    case "Audience2Scenario1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Scenario1);
                        break;
                    case "Audience2Scenario2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Scenario2);
                        break;
                    case "Audience2Scenario3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience2Scenario3);
                        break;
                    case "Audience3Scenario1":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Scenario1);
                        break;
                    case "Audience3Scenario2":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Scenario2);
                        break;
                    case "Audience3Scenario3":
                        validationResult = ValidateAudienceScenarioKeyWordFields(this.Audience3Scenario3);
                        break;
                    default:
                        throw new ApplicationException("Unknown Property being validated on Assessment.");
                }
                return validationResult;
            }
        }

        #region Field Validation
        private string ValidateFacebookUserName()
        {
            if (IsFacebook == false)
                return String.Empty;
            else
            {
                if (String.IsNullOrWhiteSpace(this.FacebookUsername))
                    return "Facebook username is required.";
                else
                    return String.Empty;
            }
        }
        private string ValidateTwitterUserName()
        {
            if (IsTwitter == false)
                return String.Empty;
            else
            {
                if (String.IsNullOrWhiteSpace(this.TwitterUsername))
                    return "Twitter username is required.";
                else
                    return String.Empty;
            }
        }
        private string ValidateYoutubeId()
        {
            if (IsYoutube == false)
                return String.Empty;
            else
            {
                if (String.IsNullOrWhiteSpace(this.YoutubeId))
                    return "Youtbe ID is required.";
                else
                    return String.Empty;
            }
        }
        private string ValidateOrganisation()
        {
            if (String.IsNullOrWhiteSpace(this.Organisation))
                return "Organisation name is required.";
            else
                return String.Empty;
        }
        private string ValidateTitle()
        {
            if (String.IsNullOrWhiteSpace(this.Title))
                return "Title is required.";
            else
                return String.Empty;
        }
        private string ValidateAudienceScenarioKeyWordFields(string content)
        {
            if (IsWeb == false)
                return String.Empty;
            else
            {
                if (String.IsNullOrWhiteSpace(content))
                    return "Field cannot be blank.";
                else
                    return String.Empty;
            }
        }
        private string ValidateUrl(string url)
        {
            if (IsWeb == false)
                return String.Empty;
            else
            {
                if (String.IsNullOrWhiteSpace(url))
                    return "Please provide a valid URL";
                else
                {
                    return String.Empty;
                }
            }
        }
        private string ValidateDateTime(DateTime? date)
        {
            if (!IsSocialMedia)
                return String.Empty;
            else
            {
                if (date==null)
                    return "Please provide a valid date";
                else
                    return String.Empty;
            }
        }


        #endregion
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
        [DefaultValue("")]
        public string link { get; set; }
        public string criteria { get; set; }
        public string recommendations { get; set; }
        public string score { get; set; }
        public string assessor { get; set; }
        public string indicator_for { get; set; }
        public List<DetailsLevel2> details_level_2 { get; set; }

        //public List<object> details_level_2 { get; set; }
    }

    public class Result
    {
        public string title { get; set; }
        public string description { get; set; }
        [DefaultValue("")]
        public string link { get; set; }
        public string score { get; set; }
        public List<DetailsLevel1> details_level_1 { get; set; }
    }

    public class RootObject
    {
        public string title { get; set; }
        public string score { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public List<Result> results { get; set; }
    }



}
