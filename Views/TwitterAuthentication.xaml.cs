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
using GalaSoft.MvvmLight.Messaging;

namespace MCIFramework.Views
{
    /// <summary>
    /// Interaction logic for TwitterAuthentication.xaml
    /// </summary>
    public partial class TwitterAuthentication : UserControl
    {
        public TwitterAuthentication()
        {
            InitializeComponent();
        }
        private void WebBrowser_Navigated_1(object sender, NavigationEventArgs e)
        {
            // FETCH URL
            Uri url = (sender as WebBrowser).Source;

            // CONVERT TO STRING IF YOU WANT IT
            //string url_as_string = url.ToString();

            // SEND TO VIEWMODEL USING MVVM LIGHT
            Messenger.Default.Send<Uri>(url, GlobalConstant.MessageTwitterBrowserChangedURL);
        }
    }
}
