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

        private Visibility _newAssessmentVisibility;
        private Visibility _createNewAssessmentVisibility;
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


        #region Constructors
        /// <summary>
        /// Create new assessment details model with default tab
        /// </summary>
        /// <param name="assessmentID"></param>
        /// <param name="tab"> 0: first tab, 1: second tab, 2: third tab</param>
        public AssessmentDetailsModel(int assessmentID, int tab)
        {
            FBAuthenEndGlobalEvent.Instance.Subscribe(FBAuthenEnd);
            _youtubeWorker.DoWork += youtubeWorker_DoWork;
            _youtubeWorker.RunWorkerCompleted += youtubeWorker_RunWorkerCompleted;
            _facebookWorker.DoWork += facebookWorker_DoWork;
            _facebookWorker.RunWorkerCompleted += facebookWorker_RunWorkerCompleted;
            _twitterWorker.DoWork += twitterWorker_DoWork;
            _twitterWorker.RunWorkerCompleted += twitterWorker_RunWorkerCompleted;

            var item = _context.assessments.FirstOrDefault(c => c.Id == assessmentID);
            this._validProperties = new Dictionary<string, bool>();
            FBAuthenCancelGlobalEvent.Instance.Subscribe(AuthenCancel);
            IsNewAssessment = false;
            DefaultTab = tab;
            CreateNewAssessmentTitle = Visibility.Hidden;
            if (item != null)
            {
                _assessment = (Assessment)item;
                if (_context.socialMediaStats.FirstOrDefault(x => x.AssessmentId == assessmentID) != null)
                    IsDownloadSocialMedialEnabled = true;
                if (IsSocialMedia)
                    IsImportSocialMediaEnabled = true;
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
                        IsImportSocialMediaEnabled=true;
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
                        Tab1MessageColor = "Red";
                    }
                    else
                    {
                        Tab1Message = "";
                        Tab1MessageColor = "Green";
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
                    Tab1MessageColor = "Green";
                    NewAssessmentCreatedGlobalEvent.Instance.Publish(_assessment);
                }
                else
                {
                    _assessment.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                    Tab1Message = "Assessment Details saved !";
                    Tab1MessageColor = "Green";
                }
            }
            catch (Exception ex)
            {
                Tab1Message = "We encountered system error. Please try to save again";
                Tab1MessageColor = "Red";
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
                _socialMediaStat.TotalDays = timeSpan.Days;
                CreateFolderAndCopyTemplate();
                if (IsYoutube)
                {
                    _youtubeWorker.RunWorkerAsync();

                }
                else
                {
                    if (IsFacebook)
                    {
                        FBAuthenGlobalEvent.Instance.Publish(_assessment);

                    }
                    else
                    {
                        if (IsTwitter)
                            _twitterWorker.RunWorkerAsync();
                        else
                        {
                            Tab2FacebookMessage = "Please select at least one social media platform";
                            Tab2FacebookMessageColor = "Red";
                            IsImportSocialMediaEnabled = true;
                        }
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
                    Tab3Message = "Please select the required files";
                    Tab3MessageColor = "Red";
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
                    client.UploadData(pathNameXLS + "Strategy Assessment.xlsx", "Post", contents);
                    Tab3Message = "Upload is completed !";
                    Tab3MessageColor = "Green";
                }
                if (LocationSocial != null)
                {
                    FileStream fStream = File.OpenRead(LocationSocial);
                    byte[] contents = new byte[fStream.Length];
                    fStream.Read(contents, 0, (int)fStream.Length);
                    fStream.Close();
                    WebClient client = new WebClient();
                    client.UploadData(pathNameXLS + "Social Media Assessment.xlsx", "Post", contents);
                    Tab3Message = "Upload is completed !";
                    Tab3MessageColor = "Green";
                }
                if (LocationWeb != null)
                {
                    FileStream fStream = File.OpenRead(LocationWeb);
                    byte[] contents = new byte[fStream.Length];
                    fStream.Read(contents, 0, (int)fStream.Length);
                    fStream.Close();
                    WebClient client = new WebClient();
                    client.UploadData(pathNameXLS + "Website Assessment.xlsx", "Post", contents);
                    Tab3Message = "Upload is completed !";
                    Tab3MessageColor = "Green";
                }

                //TO ADD MESSAGE OR NOTIFY UPLOAD IS COMPLETED
                //return "1"; //for successfull                
                //MessageBox.Show("Upload is completed");
            }
            catch (Exception ex)
            {
                Tab3Message = ex.Message;
                Tab3MessageColor = "Red";
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }

        }

        private void BrowseWorksheet(string wsName)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //set filter
            if (wsName == "Strategy")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|Strategy Assessment.xlsx";
            }
            else if (wsName == "Social")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|Social Media Assessment.xlsx";
            }
            else if (wsName == "Web")
            {
                openFileDialog1.Filter = "Excel (.xlsx)|Website Assessment.xlsx";
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
                strings += "social,";
            }
            if (_assessment.IsSocialMedia == true)
            {
                strings += "web";
            }

            //check and combine into array
            string[] myarray = strings.Split(',');
            List<Result> tempResult = new List<Result>();
            string assesNo = _assessment.Id.ToString();
            string pathName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Assessments\\";

            //loop and generate 
            foreach (string temp in myarray)
            {
                #region File names, File path etc
                string InputFileName = "";

                if (temp == "strategy")
                {
                    InputFileName = "Strategy Assessment.xlsx";
                }
                else if (temp == "social")
                {
                    InputFileName = "Social Media Assessment.xlsx";
                }
                else if (temp == "web")
                {
                    InputFileName = "Website Assessment.xlsx";
                }

                //string pathName = "F:\\Projects\\mci\\" + "Resources\\Assessments\\";

                string pathNameXLS = pathName + assesNo + "\\xlsx\\";
                string fileName = Path.Combine(pathNameXLS, InputFileName);

                string xlsTab = "";
                string xlsType = "1";
                string jsonName = "";

                if (InputFileName == "Strategy Assessment.xlsx")
                {
                    xlsTab = "SELECT * FROM [6 Data for upload$]";
                    xlsType = "1";
                    jsonName = "strategy.js";
                }
                else if (InputFileName == "Social Media Assessment.xlsx")
                {
                    xlsTab = "SELECT * FROM [10 Data for upload$]";
                    xlsType = "2";
                    jsonName = "social-media.js";
                }
                else if (InputFileName == "Website Assessment.xlsx")
                {
                    xlsTab = "SELECT * FROM [3 Data for upload$]";
                    xlsType = "2";
                    jsonName = "web.js";
                }
                #endregion
                CreateFolderAndCopyTemplate("");
                string CnStr = ("Provider=Microsoft.ACE.OLEDB.12.0;" + ("Data Source=" + (fileName + (";" + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;\""))));

                // Create the connection object 
                OleDbConnection oledbConn = new OleDbConnection(CnStr);
                try
                {
                    // Open connection
                    oledbConn.Open();

                    // Create OleDbCommand object and select data from worksheet 
                    OleDbCommand cmd = new OleDbCommand(xlsTab, oledbConn);

                    // Create new OleDbDataAdapter
                    OleDbDataAdapter oleda = new OleDbDataAdapter();

                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();

                    DataTable dt = new DataTable();
                    oleda.Fill(dt);

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
                finally
                {
                    oledbConn.Close();
                    //Tab3Message = "Report generation is completed.";
                    //Tab3MessageColor = "Green";
                    //MessageBox.Show("Report Generation is completed, click ok to start download");
                }
            }
            _assessment.ReportGenerationDate = DateTime.Now;
            _context.SaveChanges();

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

                if (fileType == "strategy")
                {
                    InputFileName = "Strategy Assessment.xlsx";
                }
                if (fileType == "social")
                {
                    InputFileName = "Social Media Assessment.xlsx";
                }
                if (fileType == "web")
                {
                    InputFileName = "Website Assessment.xlsx";
                }
                string templateFileName = Path.Combine(templatePathName, InputFileName);
                string assessmentFileName = Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", InputFileName);
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
                dlg.FileName = InputFileName; // Default file name
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
                //copy whole folder from Resources\Templates\Reports to Resources\Assessments\x\Reports
                if (!Directory.Exists(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "Report")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "Report"));
                    string _SelectedPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Templates\\Reports";
                    string destinationPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Assessments\\" + _assessment.Id.ToString() + "\\Report";
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(_SelectedPath, destinationPath);
                }

                if (!Directory.Exists(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString())))
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx"));

                if (fileType == "")
                {
                    //empty or something else, do nothing but copy report folder if it does not exist
                }

                if (fileType == "strategy")
                {
                    File.Copy(Path.Combine("Resources", "Templates", "Worksheets", "Strategy Assessment.xlsx"), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Strategy Assessment.xlsx"), true);
                }
                if (fileType == "social")
                {
                    File.Copy(Path.Combine("Resources", "Templates", "Worksheets", "Social Media Assessment.xlsx"), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Social Media Assessment.xlsx"), true);
                }
                if (fileType == "web")
                {
                    File.Copy(Path.Combine("Resources", "Templates", "Worksheets", "Website Assessment.xlsx"), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Website Assessment.xlsx"), true);
                }

            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }


        private void SaveDataToWebWorkSheet(string file)
        {
            string CnStr = ("Provider=Microsoft.ACE.OLEDB.12.0;" + ("Data Source=" + (file + (";" + "Extended Properties=\"Excel 12.0 Xml;HDR=NO;\""))));
            OleDbConnection oledbConn = new OleDbConnection(CnStr);
            try
            {
                oledbConn.Open();
                OleDbCommand command = oledbConn.CreateCommand();
                OleDbTransaction myTransaction = oledbConn.BeginTransaction();
                command.Transaction = myTransaction;

                #region hardcoded
                // First Row
                command.CommandText = "UPDATE [0 Assessment parameters$A2:B2] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", String.Empty);
                command.Parameters.AddWithValue("@2", _assessment.Organisation);
                command.ExecuteNonQuery();


                // Audience1 Kw1
                command.CommandText = "UPDATE [0 Assessment parameters$A5:C5] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience1);
                command.Parameters.AddWithValue("@2", _assessment.Audience1Scenario1);
                command.Parameters.AddWithValue("@3", _assessment.Audience1Keyword1);
                command.ExecuteNonQuery();

                // Audience1 Kw2
                command.CommandText = "UPDATE [0 Assessment parameters$A6:C6] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience1);
                command.Parameters.AddWithValue("@2", _assessment.Audience1Scenario2);
                command.Parameters.AddWithValue("@3", _assessment.Audience1Keyword2);
                command.ExecuteNonQuery();

                // Audience1 kw 3
                command.CommandText = "UPDATE [0 Assessment parameters$A7:C7] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience1);
                command.Parameters.AddWithValue("@2", _assessment.Audience1Scenario3);
                command.Parameters.AddWithValue("@3", _assessment.Audience1Keyword3);
                command.ExecuteNonQuery();


                // Audience2 Kw1
                command.CommandText = "UPDATE [0 Assessment parameters$A8:C8] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience2);
                command.Parameters.AddWithValue("@2", _assessment.Audience2Scenario1);
                command.Parameters.AddWithValue("@3", _assessment.Audience2Keyword1);
                command.ExecuteNonQuery();

                // Audience2 Kw2
                command.CommandText = "UPDATE [0 Assessment parameters$A9:C9] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience2);
                command.Parameters.AddWithValue("@2", _assessment.Audience2Scenario2);
                command.Parameters.AddWithValue("@3", _assessment.Audience2Keyword2);
                command.ExecuteNonQuery();

                // Audience2 kw 3
                command.CommandText = "UPDATE [0 Assessment parameters$A10:C10] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience2);
                command.Parameters.AddWithValue("@2", _assessment.Audience2Scenario3);
                command.Parameters.AddWithValue("@3", _assessment.Audience2Keyword3);
                command.ExecuteNonQuery();

                // Audience3 Kw1
                command.CommandText = "UPDATE [0 Assessment parameters$A11:C11] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience3);
                command.Parameters.AddWithValue("@2", _assessment.Audience3Scenario1);
                command.Parameters.AddWithValue("@3", _assessment.Audience3Keyword1);
                command.ExecuteNonQuery();

                // Audience3 Kw2
                command.CommandText = "UPDATE [0 Assessment parameters$A12:C12] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience3);
                command.Parameters.AddWithValue("@2", _assessment.Audience3Scenario2);
                command.Parameters.AddWithValue("@3", _assessment.Audience3Keyword2);
                command.ExecuteNonQuery();

                // Audience3 kw 3
                command.CommandText = "UPDATE [0 Assessment parameters$A13:C13] SET F1=@1, F2=@2, F3=@3";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.Audience3);
                command.Parameters.AddWithValue("@2", _assessment.Audience3Scenario3);
                command.Parameters.AddWithValue("@3", _assessment.Audience3Keyword3);
                command.ExecuteNonQuery();

                // Top pages
                command.CommandText = "UPDATE [0 Assessment parameters$A16:B16] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", "Homepage");
                command.Parameters.AddWithValue("@2", _assessment.WebUrl);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE [0 Assessment parameters$A17:B17] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.TopPage1);
                command.Parameters.AddWithValue("@2", _assessment.TopPageUrl1);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE [0 Assessment parameters$A18:B18] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.TopPage2);
                command.Parameters.AddWithValue("@2", _assessment.TopPageUrl2);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE [0 Assessment parameters$A19:B19] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.TopPage3);
                command.Parameters.AddWithValue("@2", _assessment.TopPageUrl3);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE [0 Assessment parameters$A20:B20] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.TopPage4);
                command.Parameters.AddWithValue("@2", _assessment.TopPageUrl4);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE [0 Assessment parameters$A21:B21] SET F1=@1, F2=@2";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@1", _assessment.TopPage5);
                command.Parameters.AddWithValue("@2", _assessment.TopPageUrl5);
                command.ExecuteNonQuery();


                #endregion

                myTransaction.Commit();
                oledbConn.Close();
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
                File.Copy(Path.Combine("Resources", "Templates", "Worksheets", "Social Media Assessment.xlsx"), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Social Media Assessment.xlsx"), true);
            }
            catch (Exception ex)
            {
                Log.LogError(this.GetType().Name + " - " + MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        #endregion

        #region Background Workers

        private void youtubeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            Tab2YoutubeMessage = "Processing Youtube data";
            Tab2YoutubeMessageColor = "Green";
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
                Tab2YoutubeMessage = "Youtube data retrieved successfully";
                Tab2YoutubeMessageColor = "Green";

            }
            else
            {
                Tab2YoutubeMessage = "Failed to retrieve Youtube data. Please try again";
                Tab2YoutubeMessageColor = "Red";
            }
            if (IsFacebook)
                FBAuthenGlobalEvent.Instance.Publish(_assessment);
            else
            {
                if (IsTwitter)
                    _twitterWorker.RunWorkerAsync();
                else// No Facebook, no Twitter
                {
                    SaveSocialMediaStat();
                }
            }
        }

        private void facebookWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            Tab2FacebookMessage = "Processing Facebook data";
            Tab2FacebookMessageColor = "Green";
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
                Tab2FacebookMessage = "Facebook data retrieved successfully";
                Tab2FacebookMessageColor = "Green";
            }
            else
            {
                if (e.Error.HResult == -2147024864)
                    Tab2FacebookMessage = "Unable to save result to Excel file. It is being used by another process)";
                
                else
                    Tab2FacebookMessage = "Failed to retrieve Facebook data. Please try again";
                Tab2FacebookMessageColor = "Red";
            }

            if (IsTwitter)
                _twitterWorker.RunWorkerAsync();
            else// no Twitter
            {
                SaveSocialMediaStat();
            }

        }

        private void twitterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            Tab2TwitterMessage = "Processing Twitter data";
            Tab2TwitterMessageColor = "Green";
            IsDownloadSocialMedialEnabled = false;
            TwitterImporter twitterImporter = new TwitterImporter(_assessment, _socialMediaStat);
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
                Tab2TwitterMessage = "Twitter data retrieved successfully";
                Tab2TwitterMessageColor = "Green";

            }
            else
            {
                Tab2TwitterMessage = "Failed to retrieve Twitter data. Please try again";
                Tab2TwitterMessageColor = "Red";
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
            if (msg == "FB Authentication completed")
            {
                Tab2FacebookMessage = msg;
                Tab2FacebookMessageColor = "Green";
                _facebookWorker.RunWorkerAsync();
            }
            else
            {
                Tab2FacebookMessage = "FB Authentication failed. Please try again";
                Tab2FacebookMessageColor = "Red";
            }
        }

        private void AuthenCancel(string msg)
        {
            if (IsSocialMedia)
                IsImportSocialMediaEnabled = true;
            
        }
        #endregion
    }
}
