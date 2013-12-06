using RedditReader.Json;
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
            RedditAPI api = new RedditAPI();
            Listing.RootObject listings = api.GetListing("/r/earthporn");

            label = new TextBlock();
            label.Text = "/r/" + listings.data.children[0].data.subreddit;
            SubredditLabels.Children.Add(label);

            foreach (var listing in listings.data.children)
            {
                if (listing.kind.ToLower().Equals("t3") && listing.data.url.ToLower().EndsWith(".jpg"))
                {
                    thumb = new Thumbnail();
                    thumb.Height = 125;
                    thumb.Width = 125;
                    thumb.ThumbnailBorder = Brushes.Black;
                    thumb.BorderThickness = new Thickness(4, 4, 4, 4);
                    thumb.Margin = new Thickness(10, 10, 10, 0);
                    thumb.ThumbnailText = listing.data.name;
                    thumb.ThumbnailBorderHighlight = Brushes.CornflowerBlue;
                    //thumb.ThumbnailUrl = new Uri(listing.data.url);
                    Thumbnails.Children.Add(thumb);
                }
            }
        }
    }
}
