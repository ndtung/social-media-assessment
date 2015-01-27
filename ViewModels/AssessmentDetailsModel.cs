using System;
using System.Collections.Generic;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks; 
using System.Windows.Input;
using System.Collections.ObjectModel;
using MCIFramework.Models;


namespace MCIFramework.ViewModels
{
    public class AssessmentDetailsModel:ViewModelBase
    {
        private Assessment _assessment;
        public AssessmentDetailsModel(int assessmentID)
        {
            Database context = new Database();
            var item = context.assessments.FirstOrDefault(c => c.Id == assessmentID);
            if (item != null)
                _assessment = (Assessment)item;
            else
            { }
        }

        public Assessment Assessment
        {
            get
            {
                return _assessment;
            }
            set
            {
                _assessment = value;
                OnPropertyChanged("Assessment");
            }
        }
       
        #region Properties
        public string Organisation
        {
            get { return _assessment.Organisation; }
            set
            {
                if (_assessment.Organisation != value)
                {
                    _assessment.Organisation = value;
                    OnPropertyChanged("Organisation");
                }
            }
        }

        public string Title
        {
            get { return _assessment.Title; }
            set
            {
                if (_assessment.Title != value)
                {
                    _assessment.Title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public string AssessmentDisplayName
        {
            get { return _assessment.Organisation+" - "+_assessment.Title; }
        }

        public DateTime? StartDate
        {
            get { return _assessment.StartDate; }
            set
            {
                if (_assessment.StartDate != value)
                {
                    _assessment.StartDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }

        public DateTime? EndDate
        {
            get { return _assessment.EndDate; }
            set
            {
                if (_assessment.EndDate != value)
                {
                    _assessment.EndDate = value;
                    OnPropertyChanged("EndDate");
                }
            }
        }

        public Boolean IsStrategy
        {
            get { return _assessment.IsStrategy; }
            set
            {
                if (_assessment.IsStrategy != value)
                {
                    _assessment.IsStrategy = value;
                    OnPropertyChanged("IsStrategy");
                }
            }
        }

        public Boolean IsSocialMedia
        {
            get { return _assessment.IsSocialMedia; }
            set
            {
                if (_assessment.IsSocialMedia != value)
                {
                    _assessment.IsSocialMedia = value;
                    OnPropertyChanged("IsSocialMedia");
                }
            }
        }
        
        public Boolean IsFacebook
        {
            get { return _assessment.IsFacebook; }
            set
            {
                if (_assessment.IsFacebook != value)
                {
                    _assessment.IsFacebook = value;
                    OnPropertyChanged("IsFacebook");
                }
            }
        }
        
        public Boolean IsYoutube
        {
            get { return _assessment.IsYoutube; }
            set
            {
                if (_assessment.IsYoutube != value)
                {
                    _assessment.IsYoutube = value;
                    OnPropertyChanged("IsYoutube");
                }
            }
        }

        public Boolean IsTwitter
        {
            get { return _assessment.IsTwitter; }
            set
            {
                if (_assessment.IsTwitter != value)
                {
                    _assessment.IsTwitter = value;
                    OnPropertyChanged("IsTwitter");
                }
            }
        }
        
        public Boolean IsWeb
        {
            get { return _assessment.IsWeb; }
            set
            {
                if (_assessment.IsWeb != value)
                {
                    _assessment.IsWeb = value;
                    OnPropertyChanged("IsWeb");
                }
            }
        }

        public string FacebookUsername
        {
            get { return _assessment.FacebookUsername; }
            set
            {
                if (_assessment.FacebookUsername != value)
                {
                    _assessment.FacebookUsername = value;
                    OnPropertyChanged("FacebookUsername");
                }
            }
        }

        public string TwitterUsername
        {
            get { return _assessment.TwitterUsername; }
            set
            {
                if (_assessment.TwitterUsername != value)
                {
                    _assessment.TwitterUsername = value;
                    OnPropertyChanged("TwitterUsername");
                }
            }
        }

        public string YoutubeId
        {
            get { return _assessment.YoutubeId; }
            set
            {
                if (_assessment.YoutubeId != value)
                {
                    _assessment.YoutubeId = value;
                    OnPropertyChanged("YoutubeId");
                }
            }
        }

        public string WebUrl
        {
            get { return _assessment.WebUrl; }
            set
            {
                if (_assessment.WebUrl != value)
                {
                    _assessment.WebUrl = value;
                    OnPropertyChanged("WebUrl");
                }
            }
        }

        public string TopPage1
        {
            get { return _assessment.TopPage1; }
            set
            {
                if (_assessment.TopPage1 != value)
                {
                    _assessment.TopPage1 = value;
                    OnPropertyChanged("TopPage1");
                }
            }
        }

        public string TopPageUrl1
        {
            get { return _assessment.TopPageUrl1; }
            set
            {
                if (_assessment.TopPageUrl1 != value)
                {
                    _assessment.TopPageUrl1 = value;
                    OnPropertyChanged("TopPageUrl1");
                }
            }
        }

        public string TopPage2
        {
            get { return _assessment.TopPage2; }
            set
            {
                if (_assessment.TopPage2 != value)
                {
                    _assessment.TopPage2 = value;
                    OnPropertyChanged("TopPage2");
                }
            }
        }

        public string TopPageUrl2
        {
            get { return _assessment.TopPageUrl2; }
            set
            {
                if (_assessment.TopPageUrl2 != value)
                {
                    _assessment.TopPageUrl2 = value;
                    OnPropertyChanged("TopPageUrl2");
                }
            }
        }

        public string TopPage3
        {
            get { return _assessment.TopPage1; }
            set
            {
                if (_assessment.TopPage3 != value)
                {
                    _assessment.TopPage3 = value;
                    OnPropertyChanged("TopPage3");
                }
            }
        }

        public string TopPageUrl3
        {
            get { return _assessment.TopPageUrl3; }
            set
            {
                if (_assessment.TopPageUrl3 != value)
                {
                    _assessment.TopPageUrl3 = value;
                    OnPropertyChanged("TopPageUrl3");
                }
            }
        }

        public string TopPage4
        {
            get { return _assessment.TopPage4; }
            set
            {
                if (_assessment.TopPage4 != value)
                {
                    _assessment.TopPage4 = value;
                    OnPropertyChanged("TopPage4");
                }
            }
        }

        public string TopPageUrl4
        {
            get { return _assessment.TopPageUrl4; }
            set
            {
                if (_assessment.TopPageUrl4 != value)
                {
                    _assessment.TopPageUrl4 = value;
                    OnPropertyChanged("TopPageUrl4");
                }
            }
        }

        public string TopPage5
        {
            get { return _assessment.TopPage5; }
            set
            {
                if (_assessment.TopPage5 != value)
                {
                    _assessment.TopPage5 = value;
                    OnPropertyChanged("TopPage5");
                }
            }
        }

        public string TopPageUrl5
        {
            get { return _assessment.TopPageUrl5; }
            set
            {
                if (_assessment.TopPageUrl5 != value)
                {
                    _assessment.TopPageUrl5 = value;
                    OnPropertyChanged("TopPageUrl5");
                }
            }
        }

        public string Audience1
        {
            get { return _assessment.Audience1; }
            set
            {
                if (_assessment.Audience1 != value)
                {
                    _assessment.Audience1 = value;
                    OnPropertyChanged("Audience1");
                }
            }
        }

        public string Audience2
        {
            get { return _assessment.Audience2; }
            set
            {
                if (_assessment.Audience2 != value)
                {
                    _assessment.Audience2 = value;
                    OnPropertyChanged("Audience2");
                }
            }
        }

        public string Audience3
        {
            get { return _assessment.Audience3; }
            set
            {
                if (_assessment.Audience3 != value)
                {
                    _assessment.Audience3 = value;
                    OnPropertyChanged("Audience3");
                }
            }
        }

        public string Audience1Scenario1
        {
            get { return _assessment.Audience1Scenario1; }
            set
            {
                if (_assessment.Audience1Scenario1 != value)
                {
                    _assessment.Audience1Scenario1 = value;
                    OnPropertyChanged("Audience1Scenario1");
                }
            }
        }

        public string Audience1Scenario2
        {
            get { return _assessment.Audience1Scenario2; }
            set
            {
                if (_assessment.Audience1Scenario2 != value)
                {
                    _assessment.Audience1Scenario2 = value;
                    OnPropertyChanged("Audience1Scenario2");
                }
            }
        }

        public string Audience1Scenario3
        {
            get { return _assessment.Audience1Scenario3; }
            set
            {
                if (_assessment.Audience1Scenario3 != value)
                {
                    _assessment.Audience1Scenario3 = value;
                    OnPropertyChanged("Audience3");
                }
            }
        }

        public string Audience2Scenario1
        {
            get { return _assessment.Audience2Scenario1; }
            set
            {
                if (_assessment.Audience2Scenario1 != value)
                {
                    _assessment.Audience2Scenario1 = value;
                    OnPropertyChanged("Audience2Scenario1");
                }
            }
        }

        public string Audience2Scenario2
        {
            get { return _assessment.Audience2Scenario2; }
            set
            {
                if (_assessment.Audience2Scenario2 != value)
                {
                    _assessment.Audience2Scenario2 = value;
                    OnPropertyChanged("Audience2Scenario2");
                }
            }
        }

        public string Audience2Scenario3
        {
            get { return _assessment.Audience2Scenario3; }
            set
            {
                if (_assessment.Audience2Scenario3 != value)
                {
                    _assessment.Audience2Scenario3 = value;
                    OnPropertyChanged("Audience2Scenario3");
                }
            }
        }

        public string Audience3Scenario1
        {
            get { return _assessment.Audience3Scenario1; }
            set
            {
                if (_assessment.Audience3Scenario1 != value)
                {
                    _assessment.Audience3Scenario1 = value;
                    OnPropertyChanged("Audience3Scenario1");
                }
            }
        }

        public string Audience3Scenario2
        {
            get { return _assessment.Audience3Scenario2; }
            set
            {
                if (_assessment.Audience3Scenario2 != value)
                {
                    _assessment.Audience3Scenario2 = value;
                    OnPropertyChanged("Audience3Scenario2");
                }
            }
        }

        public string Audience3Scenario3
        {
            get { return _assessment.Audience3Scenario3; }
            set
            {
                if (_assessment.Audience3Scenario3 != value)
                {
                    _assessment.Audience3Scenario3 = value;
                    OnPropertyChanged("Audience3Scenario3");
                }
            }
        }

        public string Audience1Keyword1
        {
            get { return _assessment.Audience1Keyword1; }
            set
            {
                if (_assessment.Audience1Keyword1 != value)
                {
                    _assessment.Audience1Keyword1 = value;
                    OnPropertyChanged("Audience1Keyword1");
                }
            }
        }

        public string Audience1Keyword2
        {
            get { return _assessment.Audience1Keyword2; }
            set
            {
                if (_assessment.Audience1Keyword2 != value)
                {
                    _assessment.Audience1Keyword2 = value;
                    OnPropertyChanged("Audience1Keyword2");
                }
            }
        }

        public string Audience1Keyword3
        {
            get { return _assessment.Audience1Keyword3; }
            set
            {
                if (_assessment.Audience1Keyword3 != value)
                {
                    _assessment.Audience1Keyword3 = value;
                    OnPropertyChanged("Audience1Keyword3");
                }
            }
        }

        public string Audience2Keyword1
        {
            get { return _assessment.Audience2Keyword1; }
            set
            {
                if (_assessment.Audience2Keyword1 != value)
                {
                    _assessment.Audience2Keyword1 = value;
                    OnPropertyChanged("Audience2Keyword1");
                }
            }
        }

        public string Audience2Keyword2
        {
            get { return _assessment.Audience2Keyword2; }
            set
            {
                if (_assessment.Audience2Keyword2 != value)
                {
                    _assessment.Audience2Keyword2 = value;
                    OnPropertyChanged("Audience2Keyword2");
                }
            }
        }

        public string Audience2Keyword3
        {
            get { return _assessment.Audience2Keyword3; }
            set
            {
                if (_assessment.Audience2Keyword3 != value)
                {
                    _assessment.Audience2Keyword3 = value;
                    OnPropertyChanged("Audience2Keyword3");
                }
            }
        }

        public string Audience3Keyword1
        {
            get { return _assessment.Audience3Keyword1; }
            set
            {
                if (_assessment.Audience3Keyword1 != value)
                {
                    _assessment.Audience3Keyword1 = value;
                    OnPropertyChanged("Audience3Keyword1");
                }
            }
        }

        public string Audience3Keyword2
        {
            get { return _assessment.Audience3Keyword2; }
            set
            {
                if (_assessment.Audience3Keyword2 != value)
                {
                    _assessment.Audience3Keyword2 = value;
                    OnPropertyChanged("Audience3Keyword2");
                }
            }
        }

        public string Audience3Keyword3
        {
            get { return _assessment.Audience3Keyword3; }
            set
            {
                if (_assessment.Audience3Keyword3 != value)
                {
                    _assessment.Audience3Keyword3 = value;
                    OnPropertyChanged("Audience3Keyword3");
                }
            }
        }

        public DateTime? ReportGenerationDate
        {
            get { return _assessment.ReportGenerationDate; }
            set
            {
                if (_assessment.ReportGenerationDate != value)
                {
                    _assessment.ReportGenerationDate = value;
                    OnPropertyChanged("ReportGenerationDate");
                }
            }
        }

        public DateTime? CreatedDate
        {
            get { return _assessment.CreatedDate; }
            set
            {
                if (_assessment.CreatedDate != value)
                {
                    _assessment.CreatedDate = value;
                    OnPropertyChanged("CreatedDate");
                }
            }
        }

        public DateTime? UpdatedDate
        {
            get { return _assessment.UpdatedDate; }
            set
            {
                if (_assessment.UpdatedDate != value)
                {
                    _assessment.UpdatedDate = value;
                    OnPropertyChanged("UpdatedDate");
                }
            }
        }

        #endregion
      

        #region Commands
        void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Assessments");
        }

        #endregion
    }
}
