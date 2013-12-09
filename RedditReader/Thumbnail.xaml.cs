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

        public static readonly DependencyProperty ThumbnailUrlProperty =
            DependencyProperty.Register("ThumbnailUrl", typeof(String), typeof(Thumbnail), new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty ThumbnailBorderProperty =
            DependencyProperty.Register("ThumbnailBorder", typeof(Brush), typeof(Thumbnail), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ThumbnailBorderHighlightProperty =
            DependencyProperty.Register("ThumbnailBorderHighlight", typeof(Brush), typeof(Thumbnail), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(bool), typeof(Thumbnail), new FrameworkPropertyMetadata(false));

        private Brush originalBorder = null;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.originalBorder == null)
                originalBorder = this.ThumbnailBorder;

            this.Selected = !this.Selected;
            Brush brush = originalBorder;
            if (this.Selected)
                brush = this.ThumbnailBorderHighlight;

            this.ThumbnailBorder = brush;

            base.OnMouseLeftButtonDown(e);
        }

        public Brush ThumbnailBorderHighlight
        {
            get { return (Brush)GetValue(ThumbnailBorderHighlightProperty); }
            set { SetValue(ThumbnailBorderHighlightProperty, value); } 
        }

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

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

        public string ThumbnailUrl
        {
            get { return (string)GetValue(ThumbnailUrlProperty); }
            set { SetValue(ThumbnailUrlProperty, value); }
        }
    }
}
