﻿using System;
using System.Collections.Generic;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks; 
using System.Windows.Input;
using System.Collections.ObjectModel;
using MCIFramework.Models;
using MCIFramework.Helper;


namespace MCIFramework.ViewModels
{
    public class AssessmentDetailsModel:ViewModelBase
    {
        private Assessment _assessment;
        private ObservableCollection<Assessment> _assessments;
        private ICommand _submitCommand;

        private ICommand _uploadWorksheet;
        private ICommand _downloadReport;
        

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

        public ObservableCollection<Assessment> Assessments
        {
            get
            {
                return _assessments;
            }
            set
            {
                _assessments = value;
                OnPropertyChanged("Assessments");
            }
        }

        public ICommand SubmitCommand
        {
            get
            {
                if (_submitCommand == null)
                {
                    _submitCommand = new RelayCommand(param => this.Submit(),
                        null);
                }
                return _submitCommand;
            }
        }
//============================================================================================================================================
        //upload Worksheets, need to check and validate if all files are uploaded before generating report
        public ICommand uploadWorksheet

        {
            get
            {
                if (_uploadWorksheet == null)
                {
                    _uploadWorksheet = new RelayCommand(param => this.Submit(),
                        null);
                }
                return _uploadWorksheet;
            }
        }

        //Download generated Report
        public ICommand downloadReport

        {
            get
            {
                if (_downloadReport == null)
                {
                    _downloadReport = new RelayCommand(param => this.Submit(),
                        null);
                }
                return _downloadReport;
            }
        }
//============================================================================================================================================
        public AssessmentDetailsModel()
        {
            Assessment = new Assessment();
            Assessments = new ObservableCollection<Assessment>();
            Assessments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);            
        }
        
        //Whenever new item is added to the collection, am explicitly calling notify property changed
        void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Assessments");
        }        
        
        
        private void Submit()
        {
            Assessment.StartDate = DateTime.Today.Date;
            Assessments.Add(Assessment);
            Assessment = new Assessment();
        }

    }


//============================================================================================================================================


//============================================================================================================================================


//============================================================================================================================================


//============================================================================================================================================


}
