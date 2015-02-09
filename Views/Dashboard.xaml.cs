using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.ComponentModel;
using MCIFramework.ViewModels;

namespace MCIFramework.Views
{
    public partial class Dashboard : UserControl
    {
        private DataGridColumn currentSortColumn;
        private ListSortDirection currentSortDirection;
        public Dashboard()
        {
            InitializeComponent();
            DashboardModel vm = new DashboardModel();
            this.DataContext = vm;
            
        }

        private void AssessmentsDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
 
            // The current sorted column must be specified in XAML.
            var currentSortColumns = dataGrid.Columns.Where(c => c.SortDirection.HasValue);
            if (currentSortColumns != null)
            {
                currentSortColumn = currentSortColumns.ToList()[0];
                currentSortDirection = currentSortColumn.SortDirection.Value;
            }
            else
            {
                currentSortColumn = dataGrid.Columns.Where(c => c.Header.ToString() == "Created Date").Single();
            }
        }
 
        /// <summary>
        /// Sets the sort direction for the current sorted column since the sort direction
        /// is lost when the DataGrid's ItemsSource property is updated.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Ignored.</param>
        private void AssessmentsDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (currentSortColumn != null)
            {
                currentSortColumn.SortDirection = currentSortDirection;
            }
        }
 
        /// <summary>
        /// Custom sort the datagrid since the actual records are stored in the
        /// server, not in the items collection of the datagrid.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Contains the column to be sorted.</param>
        private void AssessmentsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            DashboardModel mainViewModel = (DashboardModel)DataContext;
 
            string sortField = String.Empty;
 
            // Use a switch statement to check the SortMemberPath
            // and set the sort column to the actual column name. In this case,
            // the SortMemberPath and column names match.
            switch (e.Column.SortMemberPath)
            {
                case ("Organisation"):
                    sortField = "Organisation";
                    break;
                case ("Title") :
                    sortField = "Title";
                    break;
                case ("ReportGenerationDate"):
                    sortField = "ReportGenerationDate";
                    break;
                case ("CreatedDate"):
                    sortField = "CreatedDate";
                    break;
            }
 
            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ? 
                ListSortDirection.Ascending : ListSortDirection.Descending;
 
            bool sortAscending = direction == ListSortDirection.Ascending;
            mainViewModel.Sort(sortField, sortAscending);
            currentSortColumn.SortDirection = null;
            e.Column.SortDirection = direction;
            currentSortColumn = e.Column;
            currentSortDirection = direction;
        }    
    }    
}
