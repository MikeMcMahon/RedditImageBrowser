using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditReader.Json
{
    class SubReddits
    {
        public List<Subscribed> items { get; set; }

        public class Subscribed 
        {
            public string subreddit_id { get; set; }
            public string description { get; set; }
            public string subreddit_name { get; set; }
            public List<string> downloaded { get; set; }
        }
    }
}
