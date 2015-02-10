using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using MCIFramework.Models;

namespace MCIFramework.ViewModels
{
    public class MainWindowModel : BindableBase
    {
        #region Fields

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public MainWindowModel()
        {
            // Add available pages
            PageViewModels.Add(new DashboardModel());
            PageViewModels.Add(new AssessmentDetailsModel());
            PageViewModels.Add(new FBAuthenticationModel());
            PageViewModels.Add(new TwitterAuthenticationModel());

            CreateNewAssessmentGlobalEvent.Instance.Subscribe(OpenNewAssessmentViewModel);
            ToDashboardGlobalEvent.Instance.Subscribe(ToDashboardViewModel);
            EditAssessmentGlobalEvent.Instance.Subscribe(EditAssessmentViewModel);
            GenerateReportGlobalEvent.Instance.Subscribe(GenerateReportViewModel);
            FBAuthenGlobalEvent.Instance.Subscribe(ToFBAuthenViewModel);
            FBAuthenEndGlobalEvent.Instance.Subscribe(FBAuthenEnd);
            NewAssessmentCreatedGlobalEvent.Instance.Subscribe(NewAssessmentCreated);
            FBAuthenCancelGlobalEvent.Instance.Subscribe(AuthenCancel);

            TwitterAuthenGlobalEvent.Instance.Subscribe(ToTwitterAuthenViewModel);
            TwitterAuthenEndGlobalEvent.Instance.Subscribe(TwitterAuthenEnd);
            TwitterAuthenCancelGlobalEvent.Instance.Subscribe(TwitterAuthenCancel);
            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion

        #region Change page view methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        #endregion

        #region Listening to Global Events
        private void OpenNewAssessmentViewModel(string fileName)
        {
            PageViewModels[1] = new AssessmentDetailsModel();
            CurrentPageViewModel = PageViewModels[1];
        }

        private void ToDashboardViewModel(string fileName)
        {
            CurrentPageViewModel = PageViewModels[0];
        }

        private void EditAssessmentViewModel(Assessment assessment)
        {
            PageViewModels[1] = new AssessmentDetailsModel(assessment.Id,0);
            CurrentPageViewModel = PageViewModels[1];
        }

        private void GenerateReportViewModel(Assessment assessment)
        {
            PageViewModels[1] = new AssessmentDetailsModel(assessment.Id,2);
            CurrentPageViewModel = PageViewModels[1];
        }

        private void ToFBAuthenViewModel(Assessment assessment)
        {
            // Check if access token is still valid
            CurrentPageViewModel = PageViewModels[2];
        }

        private void FBAuthenEnd(String msg)
        {
            //CurrentPageViewModel = PageViewModels[1];
            //PageViewModels.Remove(PageViewModels[2]);
        }

        private void NewAssessmentCreated(Assessment assessment)
        {
            PageViewModels[1] = new AssessmentDetailsModel(assessment.Id, 1);
            CurrentPageViewModel = PageViewModels[1];
        }

        private void AuthenCancel(String msg)
        {
            if (msg == "Cancel")
            {
                CurrentPageViewModel = PageViewModels[1];
            }
        }

        private void ToTwitterAuthenViewModel(Assessment assessment)
        {
            PageViewModels[3] = new TwitterAuthenticationModel();
            CurrentPageViewModel = PageViewModels[3];
        }

        private void TwitterAuthenEnd(String msg)
        {
            CurrentPageViewModel = PageViewModels[1];
        }
        
        private void TwitterAuthenCancel(String msg)
        {
            if (msg == "Cancel")
            {
                CurrentPageViewModel = PageViewModels[1];
            }
        }

        #endregion
    }
}

 