﻿using System;
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

        private readonly BackgroundWorker _youtubeWorker = new BackgroundWorker();
        private readonly BackgroundWorker _facebookWorker = new BackgroundWorker();
        private readonly BackgroundWorker _twitterWorker = new BackgroundWorker();


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
                        IsSocialMedia = true;
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
                        IsSocialMedia = true;
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
                        IsSocialMedia = true;
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

                return _saveCommand;
            }
        }

        public ICommand DownloadWebCommand
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

                return _saveCommand;
            }
        }

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
                            DownloadSocialMediaWorksheet();
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
                }
                else
                {
                    _assessment.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                    Tab1Message = "Assessment Details saved !";
                    Tab1MessageColor = "Green";
                }
            }
            catch
            {
                Tab1Message = "We encountered system error. Please try to save again";
                Tab1MessageColor = "Red";
            }
        }

        private void DownloadStrategyWorksheet()
        {

        }

        private void DownloadWebWorksheet()
        {

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
                    }
                }

            }
        }

        private void DownloadSocialMediaWorksheet()
        {

        }

        private void UploadWorksheet()
        {

        }


        private void DownloadReport()
        {

        }

        private void ToDashboard()
        {
            ToDashboardGlobalEvent.Instance.Publish("ToDashboard");
        }

        private Boolean Validate()
        {
            return AllPropertiesValid;
        }

        private void CreateFolderAndCopyTemplate()
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString())))
                    Directory.CreateDirectory(System.IO.Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx"));
                File.Copy(Path.Combine("Resources", "Templates", "Worksheets", "Social Media Assessment.xlsx"), Path.Combine("Resources", "Assessments", _assessment.Id.ToString(), "xlsx", "Social Media Assessment.xlsx"), true);
            }
            catch
            {

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
                Tab2FacebookMessage = "Failed to retrieve Youtube data. Please try again";
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
        #endregion
    }
}
