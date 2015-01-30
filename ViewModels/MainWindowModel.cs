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
            CreateNewAssessmentGlobalEvent.Instance.Subscribe(OpenNewAssessmentViewModel);
            ToDashboardGlobalEvent.Instance.Subscribe(ToDashboardViewModel);
            EditAssessmentGlobalEvent.Instance.Subscribe(EditAssessmentViewModel);
            GenerateReportGlobalEvent.Instance.Subscribe(GenerateReportViewModel);
            
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

        private void OpenNewAssessmentViewModel(string fileName)
        {
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
    }
}

