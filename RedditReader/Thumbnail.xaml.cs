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
        }

        public static readonly DependencyProperty ThumbnailTextProperty = 
            DependencyProperty.Register("ThumbnailText", typeof(String), typeof(Thumbnail), new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty ThumbnailBorderProperty =
            DependencyProperty.Register("ThumbnailBorder", typeof(Brush), typeof(Thumbnail), new FrameworkPropertyMetadata(Brushes.Transparent));

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
    }
}
