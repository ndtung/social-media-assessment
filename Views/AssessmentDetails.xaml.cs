//using MCIFramework.Helper;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ExcelData exceldata = new ExcelData();
            //this.dataGrid1.DataContext = exceldata;
            
        }
    }
}
