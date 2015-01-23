using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCIFramework.Models;
using System.Data;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;


namespace MCIFramework.ViewModels
{
    public class DashboardModel 
    {
        private DelegateCommand _searchCommand;
        private DelegateCommand _newAssessmentCommand;
        private string _searchTextBox;
        private List<Assessment> _assessments;

        public DelegateCommand SearchCommand
        {
            get
            {
                return _searchCommand;
            }
            set
            {
                _searchCommand = value;
            }
        }
    }

}
