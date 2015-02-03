using System;
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
}
