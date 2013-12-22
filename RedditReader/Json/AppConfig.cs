using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditReader.Json
{
    class AppConfig : BindableBase
    {
        private byte[] entropy = { 1, 3, 27, 255, 23, 44, 108, 128 };
        private string _username;
        public string username { get { return _username; } set { SetProperty(ref _username, value); } }

        private string _password;
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


        public string download_directory { get; set; }
        public int reddit_pages { get; set; }
    }
}
