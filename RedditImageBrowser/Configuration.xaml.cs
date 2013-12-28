using RedditImageBrowser.Common;
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

namespace RedditImageBrowser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            
            Username.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            DownloadDir.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            RedditPages.GetBindingExpression(Slider.ValueProperty).UpdateSource();
            
            ((Config)DataContext).SaveConfig();
            
            this.Close();
        }

        private void CancelAndClose(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SelectDownloadDirectory(object sender, RoutedEventArgs e)
        {

        }
    }
}
