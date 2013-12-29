using RedditImageBrowser.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Json
{
    class AppConfig : BindableBase
    {
        private byte[] entropy = { 1, 3, 27, 255, 23, 44, 108, 128 };
        private string _username;
        private string _password;
        private string _download_directory;
        private int _reddit_pages;

        /// <summary>
        /// The username to log into reddit with
        /// </summary>
        public string username { get { return _username; } set { SetProperty(ref _username, value); } }

        /// <summary>
        /// The password to use with reddit
        /// </summary>
        public string password
        {
            get
            {
                byte[] secured = Convert.FromBase64String(_password);
                byte[] unsecured = System.Security.Cryptography.ProtectedData.Unprotect(secured, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(unsecured);
            }
            set
            {
                byte[] unsecured = Encoding.Unicode.GetBytes(value);
                byte[] secured = System.Security.Cryptography.ProtectedData.Protect(unsecured, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                SetProperty(ref _password, Convert.ToBase64String(secured));
            }
        }
        
        /// <summary>
        /// The directory to download files to
        /// </summary>
        public string download_directory { get { return _download_directory; } set { SetProperty(ref _download_directory, value); } }

        /// <summary>
        /// The directory to drop the thumbnails into
        /// </summary>
        public string thumbnail_directory { get { return Path.Combine(download_directory, "thumbs"); } }

        /// <summary>
        /// The number of reddit pages to seek
        /// </summary>
        public int reddit_pages { get { return _reddit_pages; } set { SetProperty(ref _reddit_pages, value); } }
    }
}
