using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MCIFramework.Models;
using System.ComponentModel;
using System.Windows;
using MCIFramework.Helper;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Net;
using Ionic.Zip;
using System.Reflection;
using System.Globalization;
using OfficeOpenXml;



namespace MCIFramework.ViewModels
{
    public class AssessmentDetailsModel : ViewModelBase, IDataErrorInfo, IPageViewModel
    {
        private Database _context = new Database();
        private Assessment _assessment;
        private SocialMediaStat _socialMediaStat = new SocialMediaStat();
        private Dictionary<string, bool> _validProperties;
        private bool _allPropertiesValid = false;

        private Boolean _isNewAssessment;
        private Boolean _isDownloadSocialMedialEnabled;
        private Boolean _isImportSocialMedialEnabled;

        private String _tab1Message;
        private String _tab1MessageColor;
        private String _tab3Message;
        private String _tab3MessageColor;
        private String _tab2YoutubeMessage;
        private String _tab2YoutubeMessageColor;
        private String _tab2FacebookMessage;
        private String _tab2FacebookMessageColor;
        private String _tab2TwitterMessage;
        private String _tab2TwitterMessageColor;

        private String _reportDate;

        private Visibility _newAssessmentVisibility;
        private Visibility _createNewAssessmentVisibility;
        private Visibility _youtubeLoading = Visibility.Hidden;
        private Visibility _facebookLoading = Visibility.Hidden;
        private Visibility _twitterLoading = Visibility.Hidden;
        private Visibility _twitterWarningLoadingVisible = Visibility.Hidden;

        private int _tab;

        private ICommand _uploadWorksheetCommand;
        private ICommand _downloadReportCommand;
        private ICommand _saveCommand;
        private ICommand _downloadStrategyCommand;
        private ICommand _importSocialCommand;

        private ICommand _downloadSocialCommand;
        private ICommand _downloadWebCommand;

        private ICommand _toDashboardCommand;
        private ICommand _browseStrategyCommand;
        private ICommand _browseSocialCommand;
        private ICommand _browseWebCommand;

        private readonly BackgroundWorker _youtubeWorker = new BackgroundWorker();
        private readonly BackgroundWorker _facebookWorker = new BackgroundWorker();
        private readonly BackgroundWorker _twitterWorker = new BackgroundWorker();

        private string _locationStrategy;
        private string _locationSocial;
        private string _locationWeb;

        private string _strategyExcelName;
        private string _socialExcelName;
        private string _webExcelName;

        private Boolean _facebookAuthenCompleted;
        private Boolean _twiterAuthenCompleted;
        private String _loggedInUser;
        #region Constructors
        /// <summary>
        /// Create new assessment details model with default tab
        /// </summary>
        /// <param name="assessmentID"></param>
        /// <param name="tab"> 0: first tab, 1: second tab, 2: third tab</param>
        public AssessmentDetailsModel(int assessmentID, int tab)
        {
            FBAuthenEndGlobalEvent.Instance.Subscribe(FBAuthenEnd);
            FBAuthenCancelGlobalEvent.Instance.Subscribe(AuthenCancel);

            TwitterAuthenEndGlobalEvent.Instance.Subscribe(TwitterAuthenEnd);
            TwitterAuthenCancelGlobalEvent.Instance.Subscribe(TwitterAuthenCancel);

            _youtubeWorker.DoWork += youtubeWorker_DoWork;
            _youtubeWorker.RunWorkerCompleted += youtubeWorker_RunWorkerCompleted;
            _facebookWorker.DoWork += facebookWorker_DoWork;
            _facebookWorker.RunWorkerCompleted += facebookWorker_RunWorkerCompleted;
            _twitterWorker.DoWork += twitterWorker_DoWork;
            _twitterWorker.RunWorkerCompleted += twitterWorker_RunWorkerCompleted;

            var item = _context.assessments.FirstOrDefault(c => c.Id == assessmentID);
            this._validProperties = new Dictionary<string, bool>();
            
            
            IsNewAssessment = false;
            DefaultTab = tab;
            CreateNewAssessmentTitle = Visibility.Hidden;
            if (item != null)
            {
                _assessment = (Assessment)item;
                _socialExcelName = _assessment.Organisation + " " + _assessment.Title + " "+ Properties.Resources.excel_social;
                _webExcelName = _assessment.Organisation + " " + _assessment.Title + " " + Properties.Resources.excel_web;
                _strategyExcelName = _assessment.Organisation + " " + _assessment.Title + " " + Properties.Resources.excel_strategy;

                if (_context.socialMediaStats.FirstOrDefault(x => x.AssessmentId == assessmentID) != null)
                    IsDownloadSocialMedialEnabled = true;
                if (IsSocialMedia)
                    IsImportSocialMediaEnabled = true;
                if (_assessment.ReportGenerationDate != null)
                    ReportDate = Properties.Resources.assessment_report_date + " " + _assessment.ReportGenerationDate;
                else
                    ReportDate = Properties.Resources.assessment_report_notgenerated;
            }
            else
            { new AssessmentDetailsModel(); }
        }

        public AssessmentDetailsModel()
        {
            _assessment = new Assessment();
            this._validProperties = new Dictionary<string, bool>();
            _isNewAssessment = true;
            NewAssessmentVisibility = Visibility.Hidden;
            CreateNewAssessmentTitle = Visibility.Visible;
        }

        #endregion

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
        public string ReportDate
        {
            get { return _reportDate; }
            set
            {
                if (_reportDate != value)
                {
                    _reportDate = value;
                    OnPropertyChanged("ReportDate");
                }
            }
        }

        public string Name
        {
            get { return "AssessmentDetails"; }
        }

        public string Tab1Message
        {
            get { return _tab1Message; }
            set
            {
                if (_tab1Message != value)
                {
                    _tab1Message = value;
                    OnPropertyChanged("Tab1Message");
                }
            }
        }

        public string Tab1MessageColor
        {
            get { return _tab1MessageColor; }
            set
            {
                if (_tab1MessageColor != value)
                {
                    _tab1MessageColor = value;
                    OnPropertyChanged("Tab1MessageColor");
                }
            }
        }

        public string Tab2YoutubeMessage
        {
            get { return _tab2YoutubeMessage; }
            set
            {
                if (_tab2YoutubeMessage != value)
                {
                    _tab2YoutubeMessage = value;
                    OnPropertyChanged("Tab2YoutubeMessage");
                }
            }
        }

        public string Tab2YoutubeMessageColor
        {
            get { return _tab2YoutubeMessageColor; }
            set
            {
                if (_tab2YoutubeMessageColor != value)
                {
                    _tab2YoutubeMessageColor = value;
                    OnPropertyChanged("Tab2YoutubeMessageColor");
                }
            }
        }

        public string Tab2FacebookMessage
        {
            get { return _tab2FacebookMessage; }
            set
            {
                if (_tab2FacebookMessage != value)
                {
                    _tab2FacebookMessage = value;
                    OnPropertyChanged("Tab2FacebookMessage");
                }
            }
        }

        public string Tab2FacebookMessageColor
        {
            get { return _tab2FacebookMessageColor; }
            set
            {
                if (_tab2FacebookMessageColor != value)
                {
                    _tab2FacebookMessageColor = value;
                    OnPropertyChanged("Tab2FacebookMessageColor");
                }
            }
        }

        public string Tab2TwitterMessage
        {
            get { return _tab2TwitterMessage; }
            set
            {
                if (_tab2TwitterMessage != value)
                {
                    _tab2TwitterMessage = value;
                    OnPropertyChanged("Tab2TwitterMessage");
                }
            }
        }

        public string Tab2TwitterMessageColor
        {
            get { return _tab2TwitterMessageColor; }
            set
            {
                if (_tab2TwitterMessageColor != value)
                {
                    _tab2TwitterMessageColor = value;
                    OnPropertyChanged("Tab2TwitterMessageColor");
                }
            }
        }

        public string Tab3Message
        {
            get { return _tab3Message; }
            set
            {
                if (_tab3Message != value)
                {
                    _tab3Message = value;
                    OnPropertyChanged("Tab3Message");
                }
            }
        }

        public string Tab3MessageColor
        {
            get { return _tab3MessageColor; }
            set
            {
                if (_tab3MessageColor != value)
                {
                    _tab3MessageColor = value;
                    OnPropertyChanged("Tab3MessageColor");
                }
            }
        }

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
            get { return _assessment.Organisation + " - " + _assessment.Title; }
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
                    if (value == false)
                    {
                        IsFacebook = false;
                        IsTwitter = false;
                        IsYoutube = false;
                    }
                    OnPropertyChanged("IsSocialMedia"); OnPropertyChanged("StartDate"); OnPropertyChanged("EndDate");
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
                    if (value == true)
                    {
                        IsSocialMedia = true;
                        IsImportSocialMediaEnabled = true;
                    }
                    OnPropertyChanged("IsFacebook");
                    OnPropertyChanged("FacebookUsername");

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
                    if (value == true)
                    {
                        IsSocialMedia = true;
                        IsImportSocialMediaEnabled = true;

                    }
                    OnPropertyChanged("IsYoutube");
                    OnPropertyChanged("YoutubeId");
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
                    if (value == true)
                    {
                        IsSocialMedia = true;
                        IsImportSocialMediaEnabled = true;
                    }
                    OnPropertyChanged("IsTwitter");

                    OnPropertyChanged("TwitterUsername");
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
                    OnPropertyChanged("WebUrl");
                    OnPropertyChanged("TopPage1");
                    OnPropertyChanged("TopPage2");
                    OnPropertyChanged("TopPage3");
                    OnPropertyChanged("TopPage4");
                    OnPropertyChanged("TopPage5");
                    OnPropertyChanged("TopPageUrl1");
                    OnPropertyChanged("TopPageUrl2");
                    OnPropertyChanged("TopPageUrl3");
                    OnPropertyChanged("TopPageUrl4");
                    OnPropertyChanged("TopPageUrl5");

                    OnPropertyChanged("Audience1");
                    OnPropertyChanged("Audience2");
                    OnPropertyChanged("Audience3");

                    OnPropertyChanged("Audience1Keyword1");
                    OnPropertyChanged("Audience1Keyword2");
                    OnPropertyChanged("Audience1Keyword3");

                    OnPropertyChanged("Audience2Keyword1");
                    OnPropertyChanged("Audience2Keyword2");
                    OnPropertyChanged("Audience2Keyword3");

                    OnPropertyChanged("Audience3Keyword1");
                    OnPropertyChanged("Audience3Keyword2");
                    OnPropertyChanged("Audience3Keyword3");

                    OnPropertyChanged("Audience1Scenario1");
                    OnPropertyChanged("Audience1Scenario2");
                    OnPropertyChanged("Audience1Scenario3");

                    OnPropertyChanged("Audience2Scenario1");
                    OnPropertyChanged("Audience2Scenario2");
                    OnPropertyChanged("Audience2Scenario3");

                    OnPropertyChanged("Audience3Scenario1");
                    OnPropertyChanged("Audience3Scenario2");
                    OnPropertyChanged("Audience3Scenario3");
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
            get { return _assessment.TopPage3; }
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

        public bool AllPropertiesValid
        {
            get { return _allPropertiesValid; }
            set
            {
                //  if (allPropertiesValid != value)
                {
                    _allPropertiesValid = value;
                    if (value == false)
                    {
                        Tab1Message = "Please fill in the required fields";
                        Tab1MessageColor = Properties.Resources.error_text_color;
                    }
                    else
                    {
                        Tab1Message = "";
                        Tab1MessageColor = Properties.Resources.processing_text_color;
                    }
                    base.OnPropertyChanged("AllPropertiesValid");
                }
            }
        }

        public Visibility NewAssessmentVisibility
        {
            get { return _newAssessmentVisibility; }
            set
            {
                if (_newAssessmentVisibility != value)
                {
                    _newAssessmentVisibility = value;
                    OnPropertyChanged("NewAssessmentVisibility");
                }
            }
        }

        public Visibility CreateNewAssessmentTitle
        {
            get { return _createNewAssessmentVisibility; }
            set
            {
                if (_createNewAssessmentVisibility != value)
                {
                    _createNewAssessmentVisibility = value;
                    OnPropertyChanged("CreateNewAssessmentTitle");
                }
            }
        }

        public Visibility FacebookLoadingVisible
        {
            get { return _facebookLoading; }
            set
            {
                if (_facebookLoading != value)
                {
                    _facebookLoading = value;
                    OnPropertyChanged("FacebookLoadingVisible");
                }
            }
        }

        public Visibility YoutubeLoadingVisible
        {
            get { return _youtubeLoading; }
            set
            {
                if (_youtubeLoading != value)
                {
                    _youtubeLoading = value;
                    OnPropertyChanged("YoutubeLoadingVisible");
                }
            }
        }

        public Visibility TwitterLoadingVisible
        {
            get { return _twitterLoading; }
            set
            {
                if (_twitterLoading != value)
                {
                    _twitterLoading = value;
                    OnPropertyChanged("TwitterLoadingVisible");
                }
            }
        }

        public Visibility TwitterWarningLoadingVisible
        {
            get { return _twitterWarningLoadingVisible; }
            set
            {
                if (_twitterWarningLoadingVisible != value)
                {
                    _twitterWarningLoadingVisible = value;
                    OnPropertyChanged("TwitterWarningLoadingVisible");
                }
            }
        }
        public Boolean IsNewAssessment
        {
            get { return _isNewAssessment; }
            set
            {
                if (_isNewAssessment != value)
                {
                    _isNewAssessment = value;
                    OnPropertyChanged("IsNewAssessment");
                }
            }
        }

        public Boolean IsDownloadSocialMedialEnabled
        {
            get { return _isDownloadSocialMedialEnabled; }
            set
            {
                if (_isDownloadSocialMedialEnabled != value)
                {
                    _isDownloadSocialMedialEnabled = value;
                    OnPropertyChanged("IsDownloadSocialMedialEnabled");
                }
            }
        }

        public Boolean IsImportSocialMediaEnabled
        {
            get { return _isImportSocialMedialEnabled; }
            set
            {
                if (_isImportSocialMedialEnabled != value)
                {
                    _isImportSocialMedialEnabled = value;
                    OnPropertyChanged("IsImportSocialMediaEnabled");
                }
            }
        }
        public int DefaultTab
        {
            get { return _tab; }
            set
            {
                if (_tab != value)
                {
                    _tab = value;
                    OnPropertyChanged("DefaultTab");
                }
            }
        }

        public string LocationStrategy
        {
            get { return _locationStrategy; }
            set
            {
                if (_locationStrategy != value)
                {
                    _locationStrategy = value;
                    OnPropertyChanged("LocationStrategy");
                }
            }

        }
        public string LocationSocial
        {
            get { return _locationSocial; }
            set
            {
                if (_locationSocial != value)
                {
                    _locationSocial = value;
                    OnPropertyChanged("LocationSocial");
                }
            }

        }
        public string LocationWeb
        {
            get { return _locationWeb; }
            set
            {
                if (_locationWeb != value)
                {
                    _locationWeb = value;
                    OnPropertyChanged("LocationWeb");
                }
            }

        }

        #endregion

        #region IDataErrorInfo members

        public string Error
        {
            get { return (_assessment as IDataErrorInfo).Error; }
        }

        public string this[string propertyName]
        {
            get
            {
                string error = (_assessment as IDataErrorInfo)[propertyName];
                _validProperties[propertyName] = String.IsNullOrEmpty(error) ? true : false;
                ValidateProperties();
                CommandManager.InvalidateRequerySuggested();
                //Log.LogError("IDataErrorInfo", error);
                return error;
            }
        }

        #endregion

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand
                    (
                        param =>
                        {
                            SaveAndContinue();
                        },
                        param =>
                        {
                            return Validate() == true ? true : false;
                        }
                    );
                }

                return _saveCommand;
            }
        }
        //=================================================================================================================================
        public ICommand DownloadStrategyCommand
        {
            get
            {
                if (_downloadStrategyCommand == null)
                {
                    _downloadStrategyCommand = new RelayCommand
                    (
                        param =>
                        {
                            DownloadStrategyWorksheet();
                        },
                        param =>
                        {
                            return IsStrategy == true ? true : false;
                        }
                    );
                }

                return _downloadStrategyCommand;
            }
        }

        public ICommand DownloadWebWorksheetCommand
        {
            get
            {
                if (_downloadWebCommand == null)
                {
                    _downloadWebCommand = new RelayCommand
                    (
                        param =>
                        {
                            DownloadWebWorksheet();
                        },
                        param =>
                        {
                            return IsWeb == true ? true : false;
                        }
                    );
                }

                return _downloadWebCommand;
            }
        }

        public ICommand DownloadSocialCommand
        {
            get
            {
                if (_downloadSocialCommand == null)
                {
                    _downloadSocialCommand = new RelayCommand
                    (
                        param =>
                        {
                            DownloadSocialWorksheet();
                        },
                        param =>
                        {
                            return IsSocialMedia == true ? true : false;
                        }
                    );
                }

                return _downloadSocialCommand;
            }
        }
        //=================================================================================================================================
        public ICommand ImportSocialCommand
        {
            get
            {
                if (_importSocialCommand == null)
                {
                    _importSocialCommand = new RelayCommand
                    (
                        param =>
                        {
                            ImportSocialMediaData();
                        },
                        param =>
                        {
                            return IsSocialMedia == true ? true : false;
                        }
                    );
                }

                return _importSocialCommand;
            }
        }

        public ICommand UploadWorksheetCommand
        {
            get
            {
                if (_uploadWorksheetCommand == null)
                {
                    _uploadWorksheetCommand = new RelayCommand
                    (
                         param =>
                         {
                             UploadWorksheet();
                         },
                        param =>
                        {
                            // TODO: check if the worksheet locations are specifed
                            return true;
                        }
                    );
                }
                return _uploadWorksheetCommand;
            }
        }

        public ICommand DownloadReportCommand
        {
            get
            {
                if (_downloadReportCommand == null)
                {
                    _downloadReportCommand = new RelayCommand
                    (
                        param =>
                        {
                            DownloadReport();
                        },
                        param =>
                        {
                            // TODO: check if worksheets has been uploadded
                            return true;
                        }
                    );
                }
                return _downloadReportCommand;
            }
        }

        public ICommand browseStrategyCommand
        {
            get
            {
                if (_browseStrategyCommand == null)
                {
                    _browseStrategyCommand = new RelayCommand
                    (
                        param =>
                        {
                            BrowseWorksheet("Strategy");
                        },
                        param =>
                        {
                            // TODO: check if worksheets has been uploadded
                            return true;
                        }
                    );
                }
                return _browseStrategyCommand;
            }
        }
        public ICommand browseSocialCommand
        {
            get
            {
                if (_browseSocialCommand == null)
                {
                    _browseSocialCommand = new RelayCommand
                    (
                        param =>
                        {
                            BrowseWorksheet("Social");
                        },
                        param =>
                        {
                            // TODO: check if worksheets has been uploadded
                            return true;
                        }
                    );
                }
                return _browseSocialCommand;
            }
        }
        public ICommand browseWebCommand
        {
            get
            {
                if (_browseWebCommand == null)
                {
                    _browseWebCommand = new RelayCommand
                    (
                        param =>
                        {
                            BrowseWorksheet("Web");
                        },
                        param =>
                        {
                            // TODO: check if worksheets has been uploadded
                            return true;
                        }
                    );
                }
                return _browseWebCommand;
            }
        }

        public ICommand ToDashboardCommand
        {
            get
            {
                if (_toDashboardCommand == null)
                {
                    _toDashboardCommand = new RelayCommand
                    (
                        param =>
                        {
                            ToDashboard();
                        },
                        param =>
                        {
                            // TODO: check if worksheets has been uploadded
                            return true;
                        }
                    );
                }
                return _toDashboardCommand;
            }
        }

        #endregion

        #region Actual Command Handlers

        private void ValidateProperties()
        {
            foreach (bool isValid in _validProperties.Values)
            {
                if (!isValid)
                {
                    this.AllPropertiesValid = false;
                    return;
                }
            }
            this.AllPropertiesValid = true;
        }

        private void SaveAndContinue()
        {
            try
            {
                if (_isNewAssessment)
                {
                    _assessment.CreatedDate = DateTime.Now;
                    _context.assessments.Add(_assessment);
                    _context.SaveChanges();
                    Tab1Message = "Assessment added !";
                    Tab1MessageColor = Properties.Resources.processing_text_color;
                    NewAssessmentCreatedGlobalEvent.Instance.Publish(_assessment);
                    Views.AssessmentCreated form = new Views.AssessmentCreated();
                    form.Show();
                    form.Activate();
                }
                else
                {
                    _assessment.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                    Tab1Message = "Assessment Details saved !";
                    Tab1MessageColor = Properties.Resources.processing_text_color;
                }
            }
            catch (Exception ex)
            {
                Tab1Message = "We encountered system error. Please try to save again";
                Tab1MessageColor = Properties.Resources.error_text_color;
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void DownloadStrategyWorksheet()
        {
            //CreateFolderAndCopyTemplate("strategy");
            saveLocation("strategy");

        }

        private void DownloadSocialWorksheet()
        {
            //CreateFolderAndCopyTemplate("social");
            saveLocation("social");
        }

        private void DownloadWebWorksheet()
        {
            //CreateFolderAndCopyTemplate("web");
            saveLocation("web");
        }

        private void ImportSocialMediaData()
        {
            if (IsSocialMedia)
            {
                Tab2YoutubeMessage = "";
                Tab2FacebookMessage = "";
                Tab2TwitterMessage = "";
                IsImportSocialMediaEnabled = false;
                _socialMediaStat.AssessmentId = _assessment.Id;
                TimeSpan timeSpan = (DateTime)_assessment.EndDate - (DateTime)_assessment.StartDate;
                _socialMediaStat.TotalDays = timeSpan.Days + 1;
                if (!IsFacebook && !IsTwitter && !IsYoutube)
                {
                    Tab2FacebookMessage = Properties.Resources.confirm_at_least_one_platform;
                    Tab2FacebookMessageColor = Properties.Resources.error_text_color;
                    IsImportSocialMediaEnabled = true;
                }
                else
                {
                    CreateFolderAndCopyTemplate();
                    RunAuthenticationProcess();

                }
            }
        }

        private void RunAuthenticationProcess()
        {
            if (IsFacebook)
                FBAuthenGlobalEvent.Instance.Publish(_assessment);
            else
            {
                if (IsTwitter)
                    TwitterAuthenGlobalEvent.Instance.Publish(_assessment);
                else
                {
                    if (IsYoutube)
                        StartProcessing();
                }
            }
        }

        private void StartProcessing()
        {
            // If youtube is selected, run Youtube assessment, facebook and twitter run will be triggered on youtubeworker_RunWorkerCompleted
            IsDownloadSocialMedialEnabled = false;
            IsImportSocialMediaEnabled = false;
            if (IsYoutube)
            {
                _youtubeWorker.RunWorkerAsync();
            }
            else
            {
                if (IsFacebook)
                {
                    TriggerFacebookRun();
                }
                else
                {
                    if (IsTwitter)
                    {
                        TriggerTwitterRun();
                    }
                    else
                    {
                        IsDownloadSocialMedialEnabled = true;
                        IsImportSocialMediaEnabled = true;
                    }
                }
            }
        }

        private void UploadWorksheet()
        {
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            //string pathName = "F:\\Projects\\mci\\" + "Resources\\Assessments\\";

            string pathNameXLS = pathName + assesNo + "\\xlsx\\";

            try
            {
                if (!Directory.Exists(pathNameXLS))
                {
                    Directory.CreateDirectory(pathNameXLS);
                }


                if ((IsWeb && LocationWeb == null) || (IsSocialMedia && LocationSocial == null) || (IsStrategy && LocationStrategy == null))
                {
                    Tab3Message = Properties.Resources.assessment_tab_3_select_required_files;
                    Tab3MessageColor = Properties.Resources.error_text_color;
                }
                if (LocationStrategy != null)
                {
                    //fileName = Path.Combine(pathNameXLS, LocationStrategy);                
                    //WSUploader(LocationStrategy, pathNameXLS);

                    FileStream fStream = File.OpenRead(LocationStrategy);
                    byte[] contents = new byte[fStream.Length];
                    fStream.Read(contents, 0, (int)fStream.Length);
                    fStream.Close();

                    //BinaryWriter wr = new BinaryWriter(fStream);
                    //wr.Write(reader.ReadBytes((int)Data.Length));
                    //wr.Close();

                    WebClient client = new WebClient();
                    client.UploadData(pathNameXLS + _strategyExcelName, "Post", contents);
                    Tab3Message = Properties.Resources.assessment_tab_3_upload_complete;
                    Tab3MessageColor = Properties.Resources.processing_text_color;
                }
                if (LocationSocial != null)
                {
                    FileStream fStream = File.OpenRead(LocationSocial);
                    byte[] contents = new byte[fStream.Length];
                    fStream.Read(contents, 0, (int)fStream.Length);
                    fStream.Close();
                    WebClient client = new WebClient();
                    client.UploadData(pathNameXLS + _socialExcelName, "Post", contents);
                    Tab3Message = Properties.Resources.assessment_tab_3_upload_complete;
                    Tab3MessageColor = Properties.Resources.processing_text_color;
                }
                if (LocationWeb != null)
                {
                    FileStream fStream = File.OpenRead(LocationWeb);
                    byte[] contents = new byte[fStream.Length];
                    fStream.Read(contents, 0, (int)fStream.Length);
                    fStream.Close();
                    WebClient client = new WebClient();
                    client.UploadData(pathNameXLS + _webExcelName, "Post", contents);
                    Tab3Message = Properties.Resources.assessment_tab_3_upload_complete;
                    Tab3MessageColor = Properties.Resources.processing_text_color;
                }

                //TO ADD MESSAGE OR NOTIFY UPLOAD IS COMPLETED
                //return "1"; //for successfull                
                //MessageBox.Show("Upload is completed");
            }
            catch (Exception ex)
            {
                Tab3Message = ex.Message;
                Tab3MessageColor = Properties.Resources.error_text_color;
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }

        }

        private void BrowseWorksheet(string wsName)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //set filter
            if (wsName == "Strategy")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|"+_strategyExcelName;
            }
            else if (wsName == "Social")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|"+_socialExcelName;
            }
            else if (wsName == "Web")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|"+_webExcelName;
            }

            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    if (wsName == "Strategy")
                    {

                        //WSUploader(openFileDialog1.FileName, openFileDialog1.OpenFile());
                        LocationStrategy = file;
                    }
                    else if (wsName == "Social")
                    {
                        LocationSocial = file;
                    }
                    else if (wsName == "Web")
                    {
                        LocationWeb = file;
                    }
                }
            }
            Tab3Message = "";
        }

        private void DownloadReport()
        {
            string strings = "";
            if (_assessment.IsStrategy == true)
            {
                strings += "strategy,";
            }
            if (_assessment.IsWeb == true)
            {
                strings += "web,";
            }
            if (_assessment.IsSocialMedia == true)
            {
                strings += "social";
            }

            //check and combine into array
            string[] myarray = strings.Split(',');
            List<Result> tempResult = new List<Result>();
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";
            
            CreateFolderAndCopyTemplate("");
            //loop and generate 
            foreach (string temp in myarray)
            {
                #region File names, File path etc
                string InputFileName = "";

                if (temp == "strategy")
                {
                    InputFileName = _strategyExcelName;
                }
                else if (temp == "social")
                {
                    InputFileName = _socialExcelName;
                }
                else if (temp == "web")
                {
                    InputFileName = _webExcelName;
                }

                //string pathName = "F:\\Projects\\mci\\" + "Resources\\Assessments\\";

                string pathNameXLS = pathName + assesNo + "\\xlsx\\";
                string fileName = Path.Combine(pathNameXLS, InputFileName);

                string xlsTab = "";
                string xlsType = "1";
                string jsonName = "";

                if (InputFileName == _strategyExcelName)
                {
                    xlsTab = "6 Data for upload";
                    xlsType = "1";
                    jsonName = "strategy.js";
                }
                else if (InputFileName == _socialExcelName)
                {
                    xlsTab = "10 Data for upload";
                    xlsType = "2";
                    jsonName = "social-media.js";
                }
                else if (InputFileName == _webExcelName)
                {
                    xlsTab = "3 Data for upload";
                    xlsType = "2";
                    jsonName = "web.js";
                }
                #endregion
                CreateFolderAndCopyTemplate(temp);
              
                // Create the connection object 
                
                var existingFile = new FileInfo(fileName);
                try
                {
                    DataTable dt = new DataTable();
                    using (var package = new ExcelPackage(existingFile))
                    {
                        // Get the work book in the file
                        var workBook = package.Workbook;
                        if (workBook != null)
                        {
                            if (workBook.Worksheets.Count > 0)
                            {
                                // Get the first row
                                var currentWorksheet = workBook.Worksheets[xlsTab];
                                currentWorksheet.Cells[2, 1].Value = _socialMediaStat.TotalDays;
                                bool hasHeader = true;
                                foreach (var firstRowCell in currentWorksheet.Cells[1, 1, 1, currentWorksheet.Dimension.End.Column])
                                {
                                    dt.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                                }
                                var startRow = hasHeader ? 2 : 1;
                                for (var rowNum = startRow; rowNum <= currentWorksheet.Dimension.End.Row; rowNum++)
                                {
                                    var wsRow = currentWorksheet.Cells[rowNum, 1, rowNum, currentWorksheet.Dimension.End.Column];
                                    var row = dt.NewRow();
                                    foreach (var cell in wsRow)
                                    {
                                        row[cell.Start.Column - 1] = cell.Text;
                                    }
                                    dt.Rows.Add(row);
                                }
                            }
                        }
                    }

                    RootObject root = new RootObject();
                    RootObject homeroot = new RootObject();


                    string levelNo = "";
                    List<DetailsLevel1> tempDetail1 = new List<DetailsLevel1>();
                    List<DetailsLevel2> tempDetail2 = new List<DetailsLevel2>();

                    string jsonpath = pathName + assesNo + "\\Report\\assets\\MCI-Framework\\js\\";
                    string jsonhomeItem = jsonpath + "home.js";

                    #region Read data of one file
                    foreach (DataRow dr in dt.Rows)
                    {
                        Result result = new Result();
                        DetailsLevel1 details_level_1 = new DetailsLevel1();
                        DetailsLevel2 details_level_2 = new DetailsLevel2();

                        result.details_level_1 = new List<DetailsLevel1>();
                        details_level_1.details_level_2 = new List<DetailsLevel2>();


                        if (dr.ItemArray[0].ToString().Length == 1)
                        {
                            root.organisation = _assessment.Organisation;
                            root.start_date = _assessment.StartDate == null ? "" : ((DateTime)_assessment.StartDate).ToString("d MMM yyyy", CultureInfo.InvariantCulture); ;
                            root.end_date = _assessment.EndDate == null ? "" : ((DateTime)_assessment.EndDate).ToString("d MMM yyyy", CultureInfo.InvariantCulture);
                            root.title = (dr.ItemArray[1].ToString());
                            root.score = (dr.ItemArray[2].ToString());
                            root.description = (dr.ItemArray[3].ToString());
                            root.results = new List<Result>();
                            //==========================================================================================================================================
                            ////append to home.js
                            string templink = "";
                            if (jsonName == "strategy.js")
                            {
                                templink = "2";
                            }
                            else if (jsonName == "social-media.js")
                            {
                                templink = "3";
                            }
                            else if (jsonName == "web.js")
                            {
                                templink = "4";
                            }

                            homeroot.title = Properties.Resources.homeroot_title;
                            homeroot.description = Properties.Resources.homeroot_description;
                            homeroot.score = "";
                            homeroot.link = "";
                            //homeroot.results = new List<Result>();
                            homeroot.organisation = _assessment.Organisation;
                            homeroot.start_date = _assessment.StartDate == null ? "" : ((DateTime)_assessment.StartDate).ToString("d MMM yyyy", CultureInfo.InvariantCulture); ;
                            homeroot.end_date = _assessment.EndDate == null ? "" : ((DateTime)_assessment.EndDate).ToString("d MMM yyyy", CultureInfo.InvariantCulture);
                            result.title = root.title;
                            result.score = root.score;
                            result.description = root.description;
                            result.link = templink;
                            //homeroot.results.Add(result);
                            tempResult.Add(result);
                            //==========================================================================================================================================
                        }


                        if (dr.ItemArray[0].ToString().Length == 3)
                        {
                            //result.details_level_1 = new List<DetailsLevel1>();
                            result.title = (dr.ItemArray[1].ToString());
                            result.score = (dr.ItemArray[2].ToString());
                            result.description = (dr.ItemArray[3].ToString());
                            result.link = "";
                            root.results.Add(result);
                            levelNo = dr.ItemArray[0].ToString().Substring(2, 1);

                            //reset
                            tempDetail1 = new List<DetailsLevel1>();
                            tempDetail2 = new List<DetailsLevel2>();
                        }


                        if (dr.ItemArray[0].ToString().Length == 5)
                        {
                            tempDetail2 = new List<DetailsLevel2>();
                            details_level_1.title = (dr.ItemArray[1].ToString());
                            details_level_1.score = (dr.ItemArray[2].ToString());
                            details_level_1.description = (dr.ItemArray[3].ToString());
                            details_level_1.criteria = (dr.ItemArray[4].ToString());

                            if (xlsType == "1")
                            {
                                details_level_1.recommendations = (dr.ItemArray[5].ToString());
                                details_level_1.assessor = (dr.ItemArray[6].ToString());
                            }
                            else if (xlsType == "2")
                            {
                                details_level_1.indicator_for = (dr.ItemArray[5].ToString());
                                details_level_1.recommendations = (dr.ItemArray[6].ToString());
                                details_level_1.assessor = (dr.ItemArray[7].ToString());
                            }
                            details_level_1.link = "";
                            tempDetail1.Add(details_level_1);
                        }


                        if (dr.ItemArray[0].ToString().Length == 7)
                        {
                            details_level_2.title = (dr.ItemArray[1].ToString());
                            details_level_2.score = (dr.ItemArray[2].ToString());
                            details_level_2.description = (dr.ItemArray[3].ToString());
                            details_level_2.criteria = (dr.ItemArray[4].ToString());

                            if (xlsType == "1")
                            {
                                details_level_2.recommendations = (dr.ItemArray[5].ToString());
                                details_level_2.assessor = (dr.ItemArray[6].ToString());
                            }
                            else if (xlsType == "2")
                            {
                                details_level_2.indicator_for = (dr.ItemArray[5].ToString());
                                details_level_2.recommendations = (dr.ItemArray[6].ToString());
                                details_level_2.assessor = (dr.ItemArray[7].ToString());
                            }
                            details_level_2.link = "";
                            tempDetail2.Add(details_level_2);
                            //details_level_1.details_level_2.Add(details_level_2);                            
                        }

                        result.details_level_1 = tempDetail1;
                        details_level_1.details_level_2 = tempDetail2;

                    }

                    #endregion

                    homeroot.results = tempResult;

                    //writing json file
                    //F:\Projects\mci\Resources\Assessments\1\Report\assets\MCI-Framework\js

                    string jsonItem = pathName + assesNo + "\\Report\\assets\\MCI-Framework\\js\\" + jsonName;

                    using (FileStream fs = File.Open(@jsonItem, FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(fs))
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;
                        JsonSerializer serializer = new JsonSerializer();
                        //serializer.NullValueHandling = NullValueHandling.Ignore;
                        if (jsonName == "strategy.js")
                        {
                            sw.WriteLine("var strategy = ");
                        }
                        else if (jsonName == "social-media.js")
                        {
                            sw.WriteLine("var social_media = ");
                        }
                        else if (jsonName == "web.js")
                        {
                            sw.WriteLine("var web = ");
                        }

                        serializer.Serialize(jw, root);
                        jw.Flush();
                        //fs.Position = 0;


                    }
                    //create home.js
                    using (FileStream fs1 = File.Open(@jsonhomeItem, FileMode.Create))
                    using (StreamWriter sw1 = new StreamWriter(fs1))
                    using (JsonWriter jw1 = new JsonTextWriter(sw1))
                    {
                        jw1.Formatting = Formatting.Indented;
                        JsonSerializer serializer1 = new JsonSerializer();
                        //serializer.NullValueHandling = NullValueHandling.Ignore;
                        sw1.WriteLine("var home = ");
                        serializer1.Serialize(jw1, homeroot);
                        jw1.Flush();
                        //fs.Position = 0;
                    }
                }


                catch (Exception ex)
                {
                    Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            _assessment.ReportGenerationDate = DateTime.Now;
            _context.SaveChanges();
            ReportDate = Properties.Resources.assessment_report_date + _assessment.ReportGenerationDate;
            //zip folder and ask user to save
            AddToArchive(pathName + assesNo + "\\Report\\");

        }

        private void AddToArchive(string zipFileToCreate)
        {
            using (ZipFile zip = new ZipFile())
            {
                try
                {
                    SaveFileDialog dlg = new SaveFileDialog();

                    //set default save location to mydocument
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    dlg.FileName = _assessment.Organisation + " - " + _assessment.Title; // Default file name
                    dlg.DefaultExt = ".zip"; // Default file extension
                    dlg.Filter = "Zip (.zip)|*.zip"; // Filter files by extension 

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results 
                    if (result == true)
                    {
                        // Save document                         
                        string selectpath = Path.GetDirectoryName(dlg.FileName);
                        string GetFileName = Path.GetFileName(dlg.FileName);
                        string filename = "\\" + GetFileName;

                        zip.AddDirectory(@zipFileToCreate);
                        zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                        zip.Save(@selectpath + filename);
                    }

                }
                catch (Exception ex)
                {
                    Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                }

            }

        }

        private void ToDashboard()
        {
            ToDashboardGlobalEvent.Instance.Publish("ToDashboard");
        }

        private Boolean Validate()
        {
            return AllPropertiesValid;
        }

        private void saveLocation(string fileType)
        {
            // Flow: if assessment/x//xlsx/file.xlsx is not found, copy from template folder to this folder.
            // Then copy to user selected folder 
            try
            {
                string templatePathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Templates\\Worksheets\\";
                string InputFileName = "";
                string AssessmentFile = "";
                if (fileType == "strategy")
                {
                    InputFileName = Properties.Resources.excel_strategy;
                    AssessmentFile = _strategyExcelName;
                }
                if (fileType == "social")
                {
                    InputFileName = Properties.Resources.excel_social;
                    AssessmentFile = _socialExcelName;
                }
                if (fileType == "web")
                {
                    InputFileName = Properties.Resources.excel_web;
                    AssessmentFile = _webExcelName;
                }
                string templateFileName = Path.Combine(templatePathName, InputFileName);
                string assessmentFileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", AssessmentFile);
                if (!File.Exists(assessmentFileName))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx"));
                    File.Copy(templateFileName, assessmentFileName);
                }
                if (fileType == "web")
                {
                    try
                    {
                        SaveDataToWebWorkSheet(assessmentFileName);

                    }
                    catch (Exception ex)
                    {
                        Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
                    }
                }

                SaveFileDialog dlg = new SaveFileDialog();
                //set default save location to mydocument
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.FileName = AssessmentFile; // Default file name
                dlg.Filter = "xlsx (.xlsx)|*.xlsx"; // Filter files by extension 

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results 
                if (result == true)
                {
                    // Save document                         
                    string dlgselectpath = Path.GetDirectoryName(dlg.FileName);
                    string dlgGetFileName = Path.GetFileName(dlg.FileName);
                    string userSelected = dlgselectpath + "\\" + dlgGetFileName;
                    File.Copy(assessmentFileName, userSelected, true);
                }
            }

            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }

        }


        private void CreateFolderAndCopyTemplate(string fileType)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString())))
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx"));

                if (fileType == "")
                {
                    //copy whole folder from Resources\Templates\Reports to Resources\Assessments\x\Reports
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "Report"));
                    string _SelectedPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Templates\\Reports";
                    string destinationPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Assessments\\" + _assessment.Id.ToString() + "\\Report";
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(_SelectedPath, destinationPath, true);
                }

                if (fileType == "strategy")
                {
                    if (!File.Exists(Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _strategyExcelName)))
                        File.Copy(Path.Combine("Resources", "Templates", "Worksheets", Properties.Resources.excel_strategy), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _strategyExcelName), true);
                }
                if (fileType == "social")
                {
                    if (!File.Exists(Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _socialExcelName)))
                        File.Copy(Path.Combine("Resources", "Templates", "Worksheets", Properties.Resources.excel_social), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _socialExcelName), true);
                }
                if (fileType == "web")
                {
                    if (!File.Exists(Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _webExcelName)))
                        File.Copy(Path.Combine("Resources", "Templates", "Worksheets", Properties.Resources.excel_web), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _webExcelName), true);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }


        private void SaveDataToWebWorkSheet(string file)
        {
            try
            {
                var existingFile = new FileInfo(file);
                using (var package = new ExcelPackage(existingFile))
                {
                    // Get the work book in the file
                    var workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            // Get the first row
                            var currentWorksheet = workBook.Worksheets["0 Assessment parameters"];
                            currentWorksheet.Cells[2, 2].Value = _assessment.Organisation;
                            
                            currentWorksheet.Cells[5, 1].Value = _assessment.Audience1;
                            currentWorksheet.Cells[5, 2].Value = _assessment.Audience1Scenario1;
                            currentWorksheet.Cells[5, 3].Value = _assessment.Audience1Keyword1;
                            
                            currentWorksheet.Cells[6, 1].Value = _assessment.Audience1;
                            currentWorksheet.Cells[6, 2].Value = _assessment.Audience1Scenario2;
                            currentWorksheet.Cells[6, 3].Value = _assessment.Audience1Keyword2;

                            currentWorksheet.Cells[7, 1].Value = _assessment.Audience1;
                            currentWorksheet.Cells[7, 2].Value = _assessment.Audience1Scenario3;
                            currentWorksheet.Cells[7, 3].Value = _assessment.Audience1Keyword3;

                            currentWorksheet.Cells[8, 1].Value = _assessment.Audience2;
                            currentWorksheet.Cells[8, 2].Value = _assessment.Audience2Scenario1;
                            currentWorksheet.Cells[8, 3].Value = _assessment.Audience2Keyword1;

                            currentWorksheet.Cells[9, 1].Value = _assessment.Audience2;
                            currentWorksheet.Cells[9, 2].Value = _assessment.Audience2Scenario2;
                            currentWorksheet.Cells[9, 3].Value = _assessment.Audience2Keyword2;

                            currentWorksheet.Cells[10, 1].Value = _assessment.Audience2;
                            currentWorksheet.Cells[10, 2].Value = _assessment.Audience2Scenario3;
                            currentWorksheet.Cells[11, 3].Value = _assessment.Audience2Keyword3;
                            
                            currentWorksheet.Cells[12, 1].Value = _assessment.Audience3;
                            currentWorksheet.Cells[12, 2].Value = _assessment.Audience3Scenario2;
                            currentWorksheet.Cells[12, 3].Value = _assessment.Audience3Keyword2;

                            currentWorksheet.Cells[13, 1].Value = _assessment.Audience3;
                            currentWorksheet.Cells[13, 2].Value = _assessment.Audience3Scenario3;
                            currentWorksheet.Cells[13, 3].Value = _assessment.Audience3Keyword3;

                            currentWorksheet.Cells[16, 2].Value = _assessment.WebUrl;
                            currentWorksheet.Cells[17, 1].Value = _assessment.TopPage1;
                            currentWorksheet.Cells[17, 2].Value = _assessment.TopPageUrl1;
                            currentWorksheet.Cells[18, 1].Value = _assessment.TopPage2;
                            currentWorksheet.Cells[18, 2].Value = _assessment.TopPageUrl2;
                            currentWorksheet.Cells[19, 1].Value = _assessment.TopPage3;
                            currentWorksheet.Cells[19, 2].Value = _assessment.TopPageUrl4;
                            currentWorksheet.Cells[20, 1].Value = _assessment.TopPage4;
                            currentWorksheet.Cells[20, 2].Value = _assessment.TopPageUrl4;
                            currentWorksheet.Cells[21, 1].Value = _assessment.TopPage5;
                            currentWorksheet.Cells[21, 2].Value = _assessment.TopPageUrl5;
                            package.Save();
                        }
                    }
                }
            }
                        
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }



        private void CreateFolderAndCopyTemplate()
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString())))
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx"));
                File.Copy(Path.Combine("Resources", "Templates", "Worksheets", Properties.Resources.excel_social), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", _socialExcelName), true);
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void TriggerFacebookRun()
        {
            if (_facebookAuthenCompleted)
                _facebookWorker.RunWorkerAsync();
            else
            {
                Tab2FacebookMessage = Properties.Resources.assessment_tab_2_fb_auth_failed;
                Tab2FacebookMessageColor = Properties.Resources.error_text_color;
            }
        }

        private void TriggerTwitterRun()
        {
            if (_twiterAuthenCompleted)
                _twitterWorker.RunWorkerAsync();
            else
            {
                Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_auth_failed;
                Tab2TwitterMessageColor = Properties.Resources.error_text_color;
                SaveSocialMediaStat();
            }
        }
        #endregion

        #region Background Workers

        private void youtubeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            Tab2YoutubeMessage = Properties.Resources.assessment_tab_2_processing_youtube;
            Tab2YoutubeMessageColor = Properties.Resources.processing_text_color;
            YoutubeLoadingVisible = Visibility.Visible;
            IsDownloadSocialMedialEnabled = false;
            YoutubeImporter youtubeImporter = new YoutubeImporter(_assessment, _socialMediaStat);
            youtubeImporter.Process();
            SocialMediaStat socialStat = youtubeImporter.GetDataToStore();
            _socialMediaStat.YoutubeSubscribers = socialStat.YoutubeSubscribers;
            _socialMediaStat.YoutubeVideoDislikes = socialStat.YoutubeVideoDislikes;
            _socialMediaStat.YoutubeVideoLikes = socialStat.YoutubeVideoLikes;
            _socialMediaStat.YoutubeVideos = socialStat.YoutubeVideos;
            _socialMediaStat.YoutubeVideoViews = socialStat.YoutubeVideoViews;
        }

        private void youtubeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            if (e.Error == null)
            {
                Tab2YoutubeMessage = Properties.Resources.assessment_tab_2_youtube_successful;
                Tab2YoutubeMessageColor = Properties.Resources.processing_text_color;
                YoutubeLoadingVisible = Visibility.Hidden;
            }
            else
            {
                Tab2YoutubeMessage = Properties.Resources.assessment_tab_2_youtube_failed;
                Tab2YoutubeMessageColor = Properties.Resources.error_text_color;
                YoutubeLoadingVisible = Visibility.Hidden;
            }
            if (IsFacebook)
            {
                TriggerFacebookRun();
            }
            else
            {
                if (IsTwitter)
                    TriggerTwitterRun();
                else
                    SaveSocialMediaStat();
            }
        }

        private void facebookWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            Tab2FacebookMessage = Properties.Resources.assessment_tab_2_processing_fb;
            Tab2FacebookMessageColor = Properties.Resources.processing_text_color;
            FacebookLoadingVisible = Visibility.Visible;
            IsDownloadSocialMedialEnabled = false;
            FacebookImporter facebookImporter = new FacebookImporter(_assessment, _socialMediaStat);
            facebookImporter.Process();

            SocialMediaStat socialStat = facebookImporter.GetDataToStore();
            _socialMediaStat.FacebookComments = socialStat.FacebookComments;
            _socialMediaStat.FacebookFans = socialStat.FacebookFans;
            _socialMediaStat.FacebookLikes = socialStat.FacebookLikes;
            _socialMediaStat.FacebookPosts = socialStat.FacebookPosts;
            _socialMediaStat.FacebookShares = socialStat.FacebookShares;
        }

        private void facebookWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            if (e.Error == null)
            {
                Tab2FacebookMessage = Properties.Resources.assessment_tab_2_fb_successful;
                Tab2FacebookMessageColor = Properties.Resources.processing_text_color;
                FacebookLoadingVisible = Visibility.Hidden;
            }
            else
            {
                if (e.Error.HResult == -2147024864)
                {
                    Tab2FacebookMessage = Properties.Resources.assessment_tab_2_unable_to_save;
                }
                else if (e.Error is System.OutOfMemoryException)
                {
                    Tab2FacebookMessage = Properties.Resources.assessment_tab_2_fb_out_of_memory;
                }
                else if (e.Error is Facebook.FacebookOAuthException)
                {
                    int errorCode = ((Facebook.FacebookOAuthException)e.Error).ErrorCode;
                    if (errorCode == 803)
                    {
                        Tab2FacebookMessage = Properties.Resources.assessment_tab_2_alias_not_exist;
                    }
                    else if (errorCode == 341 || errorCode == 4 || errorCode == 17 || errorCode == 2)
                    {
                        Tab2FacebookMessage = Properties.Resources.assessment_tab_2_reach_limit;
                    }
                    else if (errorCode == 463 || errorCode == 467 || errorCode == 460 || errorCode == 458)
                    {
                        Tab2FacebookMessage = Properties.Resources.assessment_tab_2_authentication_error;
                    }
                    else
                    {
                        Tab2FacebookMessage = Properties.Resources.assessment_tab_2_fb_failed;
                    }
                }

                else
                {
                    Tab2FacebookMessage = Properties.Resources.assessment_tab_2_fb_failed;
                    FacebookLoadingVisible = Visibility.Hidden;
                }
                FacebookLoadingVisible = Visibility.Hidden;
                Tab2FacebookMessageColor = Properties.Resources.error_text_color;
            }
            if (IsTwitter)
                TriggerTwitterRun();
            else// no Twitter
            {
                SaveSocialMediaStat();
            }

        }

        private void twitterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (_loggedInUser == _assessment.TwitterUsername)
            {
                Tab2TwitterMessage = Properties.Resources.assessment_tab_2_processing_twitter;
                Tab2TwitterMessageColor = Properties.Resources.processing_text_color;
                TwitterLoadingVisible = Visibility.Visible;
            }
            else
            {
                Tab2TwitterMessage = Properties.Resources.assessment_tab_2_processing_twitter_different_name_1 + _loggedInUser + Properties.Resources.assessment_tab_2_processing_twitter_different_name_2
                    + _assessment.TwitterUsername + Properties.Resources.assessment_tab_2_processing_twitter_different_name_3; 
                Tab2TwitterMessageColor = Properties.Resources.warning_text_color;
                TwitterWarningLoadingVisible = Visibility.Visible;
            }

            IsDownloadSocialMedialEnabled = false;
            TwitterImporter twitterImporter = new TwitterImporter(_assessment, _socialMediaStat,_loggedInUser);
            twitterImporter.Process();

            SocialMediaStat socialStat = twitterImporter.GetDataToStore();
            _socialMediaStat.TwitterFavourites = socialStat.TwitterFavourites;
            _socialMediaStat.TwitterFollowers = socialStat.TwitterFollowers;
            _socialMediaStat.TwitterReplies = socialStat.TwitterReplies;
            _socialMediaStat.TwitterRetweets = socialStat.TwitterRetweets;
            _socialMediaStat.TwitterTweets = socialStat.TwitterTweets;
        }

        private void twitterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            if (e.Error == null)
            {
                if (_loggedInUser==_assessment.TwitterUsername)
                    Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_successful;
                else
                    Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_successful_different_name_1+_loggedInUser+Properties.Resources.assessment_tab_2_twitter_successful_different_name_2;
                
                Tab2TwitterMessageColor = Properties.Resources.processing_text_color;
                TwitterLoadingVisible = Visibility.Hidden;
                TwitterWarningLoadingVisible = Visibility.Hidden;

            }
            else
            {
                if (e.Error is LinqToTwitter.TwitterQueryException)
                {
                    int code = ((LinqToTwitter.TwitterQueryException)e.Error).ErrorCode;
                    if (code == 429)
                        Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_reach_limit;
                    else if (code == 500 || code == 502 || code == 503 || code == 504)
                        Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_not_available;
                    else
                        Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_failed;
                }
                    
                Tab2TwitterMessage = Properties.Resources.assessment_tab_2_twitter_failed;
                Tab2TwitterMessageColor = Properties.Resources.error_text_color;
                TwitterLoadingVisible = Visibility.Hidden;
                TwitterWarningLoadingVisible = Visibility.Hidden;
            }
            SaveSocialMediaStat();
        }

        private void SaveSocialMediaStat()
        {
            _context.socialMediaStats.RemoveRange(_context.socialMediaStats.Where(x => x.AssessmentId == _assessment.Id));
            _context.socialMediaStats.Add(_socialMediaStat);
            _context.SaveChanges();
            IsDownloadSocialMedialEnabled = true;
            IsImportSocialMediaEnabled = true;
        }
        #endregion

        #region Message Passing
        private void FBAuthenEnd(string msg)
        {
            if (msg == GlobalConstant.MessagFBAuthenCompleted)
            {
                _facebookAuthenCompleted = true;
            }
            else
            {
                _facebookAuthenCompleted = false;
            }
            if (IsTwitter)
                TwitterAuthenGlobalEvent.Instance.Publish(_assessment);
            else
            {
                StartProcessing();
                ToExportWorkSheet.Instance.Publish("");
            }

        }

        private void AuthenCancel(string msg)
        {
            if (msg == GlobalConstant.Cancel)
            {
                _facebookAuthenCompleted = false;
            }
            else
            {
                _facebookAuthenCompleted = false;

            }
            if (IsTwitter)
                TwitterAuthenGlobalEvent.Instance.Publish(_assessment);
            else
            {
                StartProcessing();
                ToExportWorkSheet.Instance.Publish("");
            }

        }

        private void TwitterAuthenEnd(List<string> msg)
        {
            if (msg.Count==2)
            {
                //Tab2TwitterMessage = msg;
                //Tab2TwitterMessageColor = Properties.Resources.processing_text_color;
                _twiterAuthenCompleted = true;
                _loggedInUser = msg[1];
            }
            else
            {
                //Tab2TwitterMessage = "Twitter Authentication failed. Please try again";
                //Tab2TwitterMessageColor = Properties.Resources.error_text_color;
                _twiterAuthenCompleted = false;
            }
            StartProcessing();
        }
        private void TwitterAuthenCancel(string msg)
        {
            if (msg == GlobalConstant.Cancel)
            {
                //Tab2TwitterMessage = "Twitter Authentication cancelled";
                //Tab2TwitterMessageColor = Properties.Resources.error_text_color;
                _twiterAuthenCompleted = false;
                StartProcessing();
            }
        }
        #endregion
    }
}
