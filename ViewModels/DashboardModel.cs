using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using System.Data;
using System.Data.Entity;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MCIFramework.ViewModels
{
    public class DashboardModel : INotifyPropertyChanged, IPageViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Properties
        private Database _context = new Database();
        private string _searchBoxText = String.Empty;
        private ObservableCollection<Assessment> _assessments;
        private int _start = 0;
        private int _itemCount = Int32.Parse(Properties.Resources.value_dashboard_page_default_row_count);
        private string _sortColumn = Properties.Resources.value_dasboard_grid_default_sort_column;
        private bool _ascending = Properties.Resources.value_dasboard_grid_default_sort_direction == "ascending" ? true : false;
        private int _totalItems = 0;
        private Assessment _selectedItem;
        private InteractionRequest<Confirmation> _confirmationInteractionRequest;
        
        private ICommand _searchCommand;
        private ICommand _newAssessmentCommand;
        private ICommand _firstCommand;
        private ICommand _previousCommand;
        private ICommand _nextCommand;
        private ICommand _lastCommand;
        private ICommand _deleteAssessmentCommand;
        private ICommand _generateReportCommand;
        private ICommand _openAssessmentCommand;

        #endregion

        public DashboardModel()
        {
            RefreshAssessments();
            _confirmationInteractionRequest = new InteractionRequest<Confirmation>();
            
        }

        public string Name
        {
            get { return "Dashboard"; }
        }

        public ObservableCollection<Assessment> Assessments
        {
            get
            {
                return _assessments;
            }
            set
            {
                if (object.ReferenceEquals(_assessments, value) != true)
                {
                    _assessments = value;
                    NotifyPropertyChanged("Assessments");
                }
            }
        }

        /// <summary>
        /// Gets the index of the first item in the products list.
        /// </summary>
        public int Start { get { return _start + 1; } }

        /// <summary>
        /// Gets the index of the last item in the products list.
        /// </summary>
        public int End { get { return _start + _itemCount < _totalItems ? _start + _itemCount : _totalItems; } }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        public int TotalItems { get { return _totalItems; } }

        /// <summary>
        /// Search Text Box
        /// </summary>
        public string SearchBoxText
        {
            get
            {
                return _searchBoxText;
            }
            set
            {
                if (object.ReferenceEquals(_searchBoxText, value) != true)
                {
                    _searchBoxText = value;
                    NotifyPropertyChanged("SearchBoxText");
                }
            }
        }

        public Assessment SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// Sorts the list of products.
        /// </summary>
        /// <param name="sortColumn">The column or member that is the basis for sorting.</param>
        /// <param name="ascending">Set to true if the sort</param>
        public void Sort(string sortColumn, bool ascending)
        {
            this._sortColumn = sortColumn;
            this._ascending = ascending;

            RefreshAssessments();
        }

        public IInteractionRequest ConfirmationInteractionRequest
        {
            get { return _confirmationInteractionRequest; }
        }



        #region Commands

        public ICommand FirstCommand
        {
            get
            {
                if (_firstCommand == null)
                {
                    _firstCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start = 0;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return _start - _itemCount >= 0 ? true : false;
                        }
                    );
                }

                return _firstCommand;
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start -= _itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return _start - _itemCount >= 0 ? true : false;
                        }
                    );
                }

                return _previousCommand;
            }
        }

        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start += _itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return _start + _itemCount < _totalItems ? true : false;
                        }
                    );
                }

                return _nextCommand;
            }
        }

        public ICommand LastCommand
        {
            get
            {
                if (_lastCommand == null)
                {
                    _lastCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start = (_totalItems / _itemCount - 1) * _itemCount;
                            _start += _totalItems % _itemCount == 0 ? 0 : _itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return _start + _itemCount < _totalItems ? true : false;
                        }
                    );
                }

                return _lastCommand;
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand
                    (
                        param =>
                        {
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return SearchBoxText != null ? true : false;
                        }
                    );
                }

                return _searchCommand;
            }
        }

        public ICommand CreateNewAssessmentCommand
        {
            get
            {
                if (_newAssessmentCommand == null)
                {
                    _newAssessmentCommand = new RelayCommand
                    (
                        param =>
                        {
                            OpenNewAssessmentWindow();
                        },
                        param =>
                        {
                            return true;// Always create new
                        }
                    );
                }

                return _newAssessmentCommand;
            }
        }

        public ICommand OpenAssessmentCommand
        {
            get
            {
                if (_openAssessmentCommand == null)
                {
                    _openAssessmentCommand = new RelayCommand
                    (
                        param =>
                        {
                            OpenExistingAssessmentWindow();
                        },
                        param =>
                        {
                            return true;// Always create new
                        }
                    );
                }

                return _openAssessmentCommand;
            }
        }

        public ICommand GenerateReportCommand
        {
            get
            {
                if (_generateReportCommand == null)
                {
                    _generateReportCommand = new RelayCommand
                    (
                        param =>
                        {
                            OpenGenerateReportWindow();
                        },
                        param =>
                        {
                            return true;// Always create new
                        }
                    );
                }

                return _generateReportCommand;
            }
        }

        public ICommand DeleteAssessmentCommand
        {
            get
            {
                if (_deleteAssessmentCommand == null)
                {
                    _deleteAssessmentCommand = new RelayCommand
                    (
                        param =>
                        {
                            var result = MessageBox.Show(
                            "Are you sure you want to delete the assessment?",
                            "Confirm Delete", MessageBoxButton.OKCancel);

                            if (result.Equals(MessageBoxResult.OK))
                            {
                                DeleteSelectedItem();
                                RefreshAssessments();
                            }
                            else 
                                RefreshAssessments();

                        },
                        param =>
                        {
                            return true;
                        }
                    );
                }

                return _deleteAssessmentCommand;
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Refreshes the list of products. Called by navigation commands.
        /// </summary>
        private void RefreshAssessments()
        {
            Assessments = GetAssessments(_start, _itemCount, _sortColumn, _ascending, out _totalItems, _searchBoxText);

            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("End");
            NotifyPropertyChanged("TotalItems");
        }

        /// <summary>
        /// Notifies subscribers of changed properties.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OpenNewAssessmentWindow()
        {
            CreateNewAssessmentGlobalEvent.Instance.Publish("NewAssessment");
        }

        private void DeleteSelectedItem()
        {
            try
            {
                _context.assessments.Remove(SelectedItem);
                _context.SaveChanges();
            }
            catch
            {

            }
        }


        private void OpenExistingAssessmentWindow()
        {
            EditAssessmentGlobalEvent.Instance.Publish(SelectedItem);
        }

        private void OpenGenerateReportWindow()
        {
            GenerateReportGlobalEvent.Instance.Publish(SelectedItem);
        }

        private void ShowDialog()
        {
            _confirmationInteractionRequest.Raise(
                new Confirmation
                {
                    Title = "Confirm",
                    Content = "Are you sure you want to delete the selected assessment?"
                }, OnDialogClosed);
        }

        private void OnDialogClosed(Confirmation confirmation)
        {
            if (confirmation.Confirmed)
            {
                _context.assessments.Remove(SelectedItem);
                _context.SaveChanges();
                RefreshAssessments();
            }
            else
            {

            }
        }
        
        public bool CanDelete
        {
            get { return SelectedItem != null; }
        }
        #endregion

        #region DAL
        private ObservableCollection<Assessment> GetAssessments(int start, int itemCount, string sortColumn, bool ascending, out int totalItems, string searchBoxText)
        {
            ObservableCollection<Assessment> allAssessments = GetAllAssessments(searchBoxText);
            totalItems = allAssessments.Count;
            ObservableCollection<Assessment> sortedAssessments = new ObservableCollection<Assessment>();

            // Sort the products. In reality, the items should be stored in a database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("Organisation"):
                    sortedAssessments = new ObservableCollection<Assessment>
                    (
                        from p in allAssessments
                        orderby p.Organisation
                        select p
                    );
                    break;
                case ("Title"):
                    sortedAssessments = new ObservableCollection<Assessment>
                    (
                        from p in allAssessments
                        orderby p.Title
                        select p
                    );
                    break;
                case ("ReportGenerationDate"):
                    sortedAssessments = new ObservableCollection<Assessment>
                    (
                        allAssessments.OrderBy(row => row.ReportGenerationDate ?? DateTime.MaxValue)
                    );
                    break;
            }

            sortedAssessments = ascending ? sortedAssessments : new ObservableCollection<Assessment>(sortedAssessments.Reverse());

            ObservableCollection<Assessment> filteredAssessments = new ObservableCollection<Assessment>();

            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredAssessments.Add(sortedAssessments[i]);
            }

            return filteredAssessments;
        }

        private ObservableCollection<Assessment> GetAllAssessments(string searchBoxText)
        {
            _context.assessments.OrderByDescending(x => x.CreatedDate).Load();
            ObservableCollection<Assessment> result = _context.assessments.Local;
            var result2 = from s in result
                          where s.Organisation.ToLower().Contains(searchBoxText.ToLower()) ||
                                s.Title.ToLower().Contains(searchBoxText.ToLower())
                          select s;

            return new ObservableCollection<Assessment>(result2);

        }
        #endregion


    }

}
