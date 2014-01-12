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
        private static byte[] entropy = { 1, 3, 27, 255, 23, 44, 108, 128 };
        private string _username;
        private string _password;
        private string _download_directory;
        private int _reddit_pages;
        private string _modhash;

        /// <summary>
        /// The username to log into reddit with
        /// </summary>
        public string username { get { return _username; } set { SetProperty(ref _username, value); } }

        
        /// <summary>
        /// Gets or sets the modhash for the user, the modhash is essentially the logon hash
        /// </summary>
        public string modhash
        {
            get
            {
                if (_modhash == null)
                    return "";

                return _modhash;
            }
            set
            {
                if (value == null || value.Equals("")) 
                    SetProperty(ref _modhash, "");
                else
                    SetProperty(ref _modhash, Encrypt(value));
            }
        }

        /// <summary>
        /// The password to use with reddit
        /// </summary>
        public string password
        {
            get
            {
                if (_password == null)
                    return "";

                return _password;
            }
            set
            {
                if (value == null || value.Equals(""))
                    SetProperty(ref _password, "");
                else
                    SetProperty(ref _password, Encrypt(value));
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

        public string cookie { get; set; }

        #region Weakest Encraption Ever
        /// <summary>
        /// Decrypts an encrypted string
        /// </summary>
        /// <param name="crypt"></param>
        /// <returns></returns>
        public static string Decrypt(string crypt)
        {
            if (crypt == null || crypt.Equals(""))
                return "";

            byte[] secured = Convert.FromBase64String(crypt);
            byte[] unsecured = System.Security.Cryptography.ProtectedData.Unprotect(secured, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(unsecured);
        }

        /// <summary>
        /// Encrypt a string (well use the current user encryption...meh)
        /// </summary>
        /// <param name="toCrypt"></param>
        /// <returns></returns>
        private static string Encrypt(string toCrypt)
        {
            byte[] unsecured = Encoding.Unicode.GetBytes(toCrypt);
            byte[] secured = System.Security.Cryptography.ProtectedData.Protect(unsecured, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(secured);
        }
        #endregion
    }
}
