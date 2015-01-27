using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using System.Data;
using System.Data.Entity;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace MCIFramework.ViewModels
{
    public class DashboardModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Properties

        private string searchBoxText = String.Empty;
        private ObservableCollection<Assessment> assessments;
        private int start = 0;
        private int itemCount = Int32.Parse(Properties.Resources.value_dashboard_page_default_row_count);
        private string sortColumn = Properties.Resources.value_dasboard_grid_default_sort_column;
        private bool ascending = Properties.Resources.value_dasboard_grid_default_sort_direction == "ascending" ? true:false;
        private int totalItems = 0;

        private ICommand searchCommand;
        private ICommand newAssessmentCommand;
        private ICommand firstCommand;
        private ICommand previousCommand;
        private ICommand nextCommand;
        private ICommand lastCommand;
        
        #endregion

        public DashboardModel()
        {

            RefreshAssessments();
        }

        public ObservableCollection<Assessment> Assessments
        {
            get
            {
                return assessments;
            }
            set
            {
                if (object.ReferenceEquals(assessments, value) != true)
                {
                    assessments = value;
                    NotifyPropertyChanged("Assessments");
                }
            }
        }

        /// <summary>
        /// Gets the index of the first item in the products list.
        /// </summary>
        public int Start { get { return start + 1; } }

        /// <summary>
        /// Gets the index of the last item in the products list.
        /// </summary>
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        public int TotalItems { get { return totalItems; } }

        /// <summary>
        /// Search Text Box
        /// </summary>
        public string SearchBoxText 
        { 
            get 
            { 
                return searchBoxText; 
            }
            set
            {
                if (object.ReferenceEquals(searchBoxText, value) != true)
                {
                    searchBoxText = value;
                    NotifyPropertyChanged("SearchBoxText");
                }
            }
        }

       

        /// <summary>
        /// Sorts the list of products.
        /// </summary>
        /// <param name="sortColumn">The column or member that is the basis for sorting.</param>
        /// <param name="ascending">Set to true if the sort</param>
        public void Sort(string sortColumn, bool ascending)
        {
            this.sortColumn = sortColumn;
            this.ascending = ascending;

            RefreshAssessments();
        }

        /// <summary>
        /// Refreshes the list of products. Called by navigation commands.
        /// </summary>
        private void RefreshAssessments()
        {
            Assessments = GetAssessments(start, itemCount, sortColumn, ascending, out totalItems, searchBoxText);

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

        #region Commands

        /// <summary>
        /// Gets the command for moving to the first page of assessments.
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new RelayCommand
                    (
                        param =>
                        {
                            start = 0;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }

                return firstCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the previous page of assessments.
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new RelayCommand
                    (
                        param =>
                        {
                            start -= itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }

                return previousCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the next page of assessments.
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new RelayCommand
                    (
                        param =>
                        {
                            start += itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return nextCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the last page of assessments.
        /// </summary>
        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new RelayCommand
                    (
                        param =>
                        {
                            start = (totalItems / itemCount - 1) * itemCount;
                            start += totalItems % itemCount == 0 ? 0 : itemCount;
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return lastCommand;
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new RelayCommand
                    (
                        param =>
                        {
                            RefreshAssessments();
                        },
                        param =>
                        {
                            return SearchBoxText !=null ? true : false;
                        }
                    );
                }

                return searchCommand;
            }
        }

        public ICommand CreateNewAssessmentCommand
        {
            get
            {
                if (newAssessmentCommand == null)
                {
                    newAssessmentCommand = new RelayCommand
                    (
                        param =>
                        {
                            
                        },
                        param =>
                        {
                            return true;// Always create new
                        }
                    );
                }

                return newAssessmentCommand;
            }
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
            Database context = new Database();
            context.assessments.OrderByDescending(x => x.CreatedDate).Load();
            ObservableCollection<Assessment> result = context.assessments.Local;
            var result2 = from s in result
                          where s.Organisation.ToLower().Contains(searchBoxText.ToLower()) ||
                                s.Title.ToLower().Contains(searchBoxText.ToLower())
                          select s;
            
            return new ObservableCollection<Assessment>(result2);
        }

        #endregion



    }

}
