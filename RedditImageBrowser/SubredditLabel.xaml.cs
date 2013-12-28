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

namespace RedditImageBrowser
{
    /// <summary>
    /// Interaction logic for SubredditLabel.xaml
    /// </summary>
    public partial class SubredditLabel : UserControl
    {
        public SubredditLabel()
        {
            InitializeComponent();
            Remove.Click += Remove_Click;
        }

        void Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClicked(this, new RemoveClickedEventArgs());
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SubredditLabel), new FrameworkPropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #region RemoveEvent
        public event EventHandler<RemoveClickedEventArgs> RemoveClicked;
        public virtual void OnRemoveClicked(RemoveClickedEventArgs e)
        {
            if (RemoveClicked != null)
                RemoveClicked(this, e);
        }
        public class RemoveClickedEventArgs : EventArgs
        {
            public RemoveClickedEventArgs() { }
        }
        #endregion
    }
}
