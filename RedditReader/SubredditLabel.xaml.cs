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

namespace RedditReader
{
    /// <summary>
    /// Interaction logic for SubredditLabel.xaml
    /// </summary>
    public partial class SubredditLabel : UserControl
    {
        public SubredditLabel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SubredditLabel), new FrameworkPropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Fill = Brushes.Gray;
        }
        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Rectangle)sender).Fill = Brushes.Black;
        }
    }
}
