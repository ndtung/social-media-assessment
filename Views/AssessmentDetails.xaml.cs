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
using System.Windows.Shapes;
using MCIFramework.Models;
using System.Collections.Generic;
using MCIFramework.ViewModels;

namespace MCIFramework.Views
{
    /// <summary>
    /// Interaction logic for AssessmentDetails.xaml
    /// </summary>
    public partial class AssessmentDetails : Window
    {
        public AssessmentDetails()
        {
            InitializeComponent();
            AssessmentDetailsModel vm = new AssessmentDetailsModel(1);
            this.DataContext = vm;
            
        }
    }
}
