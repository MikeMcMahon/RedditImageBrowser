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
    /// Interaction logic for Thumbnail.xaml
    /// </summary>
    public partial class Thumbnail : UserControl
    {
        public Thumbnail()
        {
            InitializeComponent();

            this.ThumbnailUrl = new Uri("pack://application:,,,/Assets/Loading.png");
        }

        public static readonly DependencyProperty ThumbnailTextProperty = 
            DependencyProperty.Register("ThumbnailText", typeof(String), typeof(Thumbnail), new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty ThumbnailUrlProperty =
            DependencyProperty.Register("ThumbnailUrl", typeof(Uri), typeof(Thumbnail), new FrameworkPropertyMetadata(new Uri("pack://application:,,,/Assets/Loading.png")));

        public static readonly DependencyProperty ThumbnailBorderProperty =
            DependencyProperty.Register("ThumbnailBorder", typeof(Brush), typeof(Thumbnail), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ThumbnailBorderHighlightProperty =
            DependencyProperty.Register("ThumbnailBorderHighlight", typeof(Brush), typeof(Thumbnail), new FrameworkPropertyMetadata(Brushes.Transparent));

        private Brush borderOrig = null;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (borderOrig == null)
                borderOrig = this.ThumbnailBorder;
            this.ThumbnailBorder = this.ThumbnailBorderHighlight;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ThumbnailBorder = borderOrig;
        }

        public Brush ThumbnailBorderHighlight
        {
            get { return (Brush)GetValue(ThumbnailBorderHighlightProperty); }
            set { SetValue(ThumbnailBorderHighlightProperty, value); } 
        }/**/

        public Brush ThumbnailBorder
        {
            get { return (Brush)GetValue(ThumbnailBorderProperty); }
            set { SetValue(ThumbnailBorderProperty, value); }
        }

        public string ThumbnailText
        {
            get { return GetValue(ThumbnailTextProperty).ToString(); }
            set { SetValue(ThumbnailTextProperty, value); }
        }

        public Uri ThumbnailUrl
        {
            get { return (Uri)GetValue(ThumbnailUrlProperty); }
            set { SetValue(ThumbnailUrlProperty, value); }
        }
    }
}
