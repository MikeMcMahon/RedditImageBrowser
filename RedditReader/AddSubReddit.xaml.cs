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

namespace RedditReader
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddSubReddit : Window
    {
        public AddSubReddit()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #region Validate Sub Reddit Entered
        private void SubRedditText_KeyDown(object sender, KeyEventArgs e)
        {
            ValidateInput();
        }

        private void SubRedditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void SubRedditText_TextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateInput();
        }

        bool ValidateInput()
        {
            if (!this.SubRedditText.Text.StartsWith("/r/"))
            {
                Color lightRed = new Color();
                lightRed.R = 255;
                lightRed.A = 60;
                SolidColorBrush br = new SolidColorBrush(lightRed);
                this.SubRedditText.Background = br;
                return false;
            }
            else
            {
                this.SubRedditText.Background = Brushes.White;
                return true;
            }
        }
        #endregion
    }
}
