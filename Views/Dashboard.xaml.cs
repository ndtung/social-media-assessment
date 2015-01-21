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

namespace MCIFramework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NorthwindContext context = new NorthwindContext();
                context.FbPosts.OrderBy(c => c.ID).Load();
                this.dataGrid.ItemsSource = context.FbPosts.Local;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }
    }
    
    /// <summary>6
    /// Entity framework context
    /// </summary>
    public class NorthwindContext : DbContext
    {
        public DbSet<FacebookPost> FbPosts { get; set; }
    }

    /// <summary>
    /// Employee class
    /// </summary>
    public class FacebookPost
    {
        public int ID { get; set; }
        public int FacebookId { get; set; }
        public string Post { get; set; }
    }
}
