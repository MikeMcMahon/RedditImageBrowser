using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Json
{
    class Login : JsonApi
    {
        public class Data
        {
            public string modhash { get; set; }
            public string cookie { get; set; }
        }

        public class Json
        {
            public List<object> errors { get; set; }
            public Data data { get; set; }
        }

        public Json json { get; set; }
    }
}
