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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TextBlock label;
            Thumbnail thumb;
            for (int i = 0; i < 100; i++)
            {
                label = new TextBlock();
                label.Text = "/r/Earthporn";
                SubredditLabels.Children.Add(label);
                thumb = new Thumbnail();
                thumb.Width = 100;
                thumb.Height = 100;
                thumb.Background = Brushes.Black;
                Thumbnails.Children.Add(thumb);
            }
        }
    }
}
