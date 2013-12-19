using RedditReader.Json;
using RedditReader.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            SubredditLabel label;
            Thumbnail thumb;
            RedditAPI api = new RedditAPI();
            Listing.RootObject listings = api.GetListing("/r/earthporn");

            label = new SubredditLabel();
            label.Text = "/r/" + listings.data.children[0].data.subreddit;
            label.Height = 35;
            label.Width = 150;
            label.Margin = new Thickness(0, 10, 0, 10);
            SubredditLabels.Children.Add(label);
            DownloadManager dm = new DownloadManager(10);
            dm.Start();
            foreach (var listing in listings.data.children) {
                if (listing.kind.ToLower().Equals("t3") & 
                    (listing.data.url.ToLower().EndsWith(".jpg") | listing.data.url.ToLower().EndsWith(".png") |
                    listing.data.url.ToLower().EndsWith(".jpeg"))) {
                    thumb = new Thumbnail();
                    thumb.Height = 125;
                    thumb.Width = 110;
                    thumb.ThumbnailBorder = Brushes.Black;
                    thumb.BorderThickness = new Thickness(4, 4, 4, 4);
                    thumb.Margin = new Thickness(10, 10, 10, 0);
                    thumb.ThumbnailText = listing.data.name;
                    thumb.ThumbnailBorderHighlight = Brushes.LightGreen;
                    thumb.MouseLeftButtonDown += thumb_MouseLeftButtonDown;
                    thumb.SetDetails(
                        listing.data.title, 
                        listing.data.score, 
                        listing.data.ups, 
                        listing.data.downs, 
                        listing.data.permalink
                    );
                    
                    dm.AddDownload(new Uri(listing.data.thumbnail), @"D:\Development\thumbs\" + listing.data.name + ".jpg");

                    thumb.ThumbnailUrl = listing.data.thumbnail;
                    //@"D:\Development\thumbs\" + listing.data.name + ".jpg";
                    Thumbnails.Children.Add(thumb);
                }
            }

            //Configuration config = new Configuration();
            //config.Show();
        }
        BitmapImage img = null;
        void thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thumbnail thumb = (Thumbnail)sender;
            if (thumb.Selected)
                thumb.Selected = false;
            else
                thumb.Selected = true;

            if (img != null)
                img = null;

            /*img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(@"D:\Development\thumbs\" + thumb.ThumbnailText + ".jpg");
            img.DecodePixelWidth = 500;
            img.EndInit();
            Preview.Source = img;/**/
            foreach (Thumbnail elem in Thumbnails.Children.Cast<Thumbnail>())
            {
                if (!thumb.ThumbnailText.Equals(elem.ThumbnailText))
                    elem.Selected = false;
            }
        }
    }
}
