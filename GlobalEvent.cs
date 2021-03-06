﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;
using MCIFramework.Models;


namespace MCIFramework
{
    internal class CreateNewAssessmentGlobalEvent : PubSubEvent<string>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly CreateNewAssessmentGlobalEvent _event;

        static CreateNewAssessmentGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<CreateNewAssessmentGlobalEvent>();
        }

        public static CreateNewAssessmentGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class ToDashboardGlobalEvent : PubSubEvent<string>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly ToDashboardGlobalEvent _event;

        static ToDashboardGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<ToDashboardGlobalEvent>();
        }

        public static ToDashboardGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class EditAssessmentGlobalEvent : PubSubEvent<Assessment>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly EditAssessmentGlobalEvent _event;

        static EditAssessmentGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<EditAssessmentGlobalEvent>();
        }

        public static EditAssessmentGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class GenerateReportGlobalEvent : PubSubEvent<Assessment>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly GenerateReportGlobalEvent _event;

        static GenerateReportGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<GenerateReportGlobalEvent>();
        }

        public static GenerateReportGlobalEvent Instance
        {
            get { return _event; }
        }
    }
    
    internal class NewAssessmentCreatedGlobalEvent : PubSubEvent<Assessment>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly NewAssessmentCreatedGlobalEvent _event;

        static NewAssessmentCreatedGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<NewAssessmentCreatedGlobalEvent>();
        }

        public static NewAssessmentCreatedGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class ToExportWorkSheet : PubSubEvent<String>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly ToExportWorkSheet _event;

        static ToExportWorkSheet()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<ToExportWorkSheet>();
        }

        public static ToExportWorkSheet Instance
        {
            get { return _event; }
        }
    }

    internal class FBAuthenGlobalEvent : PubSubEvent<Assessment>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly FBAuthenGlobalEvent _event;

        static FBAuthenGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<FBAuthenGlobalEvent>();
        }

        public static FBAuthenGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class FBAuthenEndGlobalEvent : PubSubEvent<String>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly FBAuthenEndGlobalEvent _event;

        static FBAuthenEndGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<FBAuthenEndGlobalEvent>();
        }

        public static FBAuthenEndGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class FBAuthenCancelGlobalEvent : PubSubEvent<String>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly FBAuthenCancelGlobalEvent _event;

        static FBAuthenCancelGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<FBAuthenCancelGlobalEvent>();
        }

        public static FBAuthenCancelGlobalEvent Instance
        {
            get { return _event; }
        }
    }


    internal class TwitterAuthenGlobalEvent : PubSubEvent<Assessment>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly TwitterAuthenGlobalEvent _event;

        static TwitterAuthenGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<TwitterAuthenGlobalEvent>();
        }

        public static TwitterAuthenGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class TwitterAuthenEndGlobalEvent : PubSubEvent<List<String>>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly TwitterAuthenEndGlobalEvent _event;

        static TwitterAuthenEndGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<TwitterAuthenEndGlobalEvent>();
        }

        public static TwitterAuthenEndGlobalEvent Instance
        {
            get { return _event; }
        }
    }

    internal class TwitterAuthenCancelGlobalEvent : PubSubEvent<String>
    {
        private static readonly EventAggregator _eventAggregator;
        private static readonly TwitterAuthenCancelGlobalEvent _event;

        static TwitterAuthenCancelGlobalEvent()
        {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<TwitterAuthenCancelGlobalEvent>();
        }

        public static TwitterAuthenCancelGlobalEvent Instance
        {
            get { return _event; }
        }
    }
}
