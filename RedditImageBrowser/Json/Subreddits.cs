using RedditImageBrowser.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Json
{
    public class Subscribed : BindableBase
    {
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public List<string> downloaded { get; set; }
    }

    class Subreddits : ObservableCollection<Subscribed> // ICollection<Subscribed>
    {
        public Subreddits()
        {
        }
    }
}
