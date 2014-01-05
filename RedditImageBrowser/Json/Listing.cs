using RedditImageBrowser.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Json
{
    class Listing
    {
        public static Listing EmptyListing()
        {
            Listing l = new Listing();
            l.data = new Data();
            l.data.children = new ObservableCollection<Child>();
            return l;
        }

        public string kind { get; set; }
        public Data data { get; set; }

        public static bool IsSupportedFormat(Uri url)
        {
            foreach (string ext in Config.supporfted_file_formats.ToList<string>()) {
                if (url.AbsolutePath.ToLower().EndsWith(ext))
                    return true;
            }

            return false;
        }
        public class MediaEmbed
        {
        }

        public class SecureMediaEmbed
        {
        }

        public class Details
        {
            public string domain { get; set; }
            public object banned_by { get; set; }
            public MediaEmbed media_embed { get; set; }
            public string subreddit { get; set; }
            public string selftext_html { get; set; }
            public string selftext { get; set; }
            public object likes { get; set; }
            public object secure_media { get; set; }
            public object link_flair_text { get; set; }
            public string id { get; set; }
            public SecureMediaEmbed secure_media_embed { get; set; }
            public bool clicked { get; set; }
            public bool stickied { get; set; }

            private string _author;
            public string author { get { return "/u/" + _author; } set { _author = value; } }
            public object media { get; set; }
            public int score { get; set; }
            public object approved_by { get; set; }
            public bool over_18 { get; set; }
            public bool hidden { get; set; }
            public string thumbnail { get; set; }
            public string subreddit_id { get; set; }
            public object edited { get; set; }
            public object link_flair_css_class { get; set; }
            public string author_flair_css_class { get; set; }
            public int downs { get; set; }
            public bool saved { get; set; }
            public bool is_self { get; set; }
            public string permalink { get; set; }
            public string name { get; set; }
            public double created { get; set; }
            public string url { get; set; }
            public object author_flair_text { get; set; }
            public string title { get; set; }
            public double created_utc { get; set; }
            public int ups { get; set; }
            public int num_comments { get; set; }
            public bool visited { get; set; }
            public object num_reports { get; set; }
            public string distinguished { get; set; }
        }

        public class Child
        {
            public string kind { get; set; }
            public Details data { get; set; }
        }

        public class Data
        {
            public string modhash { get; set; }
            public ObservableCollection<Child> children { get; set; }
            // spublic List<Child> children { get; set; }
            public string after { get; set; }
            public string before { get; set; }
        }
    }
}
