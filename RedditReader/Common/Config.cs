using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditReader.Common
{
    class Config
    {
        public class Directories
        {
            private static string _Thumbnails = null;
            public static string Thumbnails
            {
                get
                {
                    if (_Thumbnails == null)
                        _Thumbnails = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return _Thumbnails;
                }
                set { _Thumbnails = value; }
            }
        }

        public readonly string[] supported_file_formats = { ".jpeg", ".jpg", ".bmp", ".png" };
    }
}
