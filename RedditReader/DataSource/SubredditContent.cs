using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditReader.DataSource
{
    class SubredditContent
    {
        public ObservableCollection<RedditReader.Json.Listing.Child> Content { get; set; }
        public SubredditContent()
        {
            Content = new ObservableCollection<Json.Listing.Child>();
            /*Content.Add(new Json.Listing.Child());
            Content[0].data = new Json.Listing.Details();
            Content[0].data.url = "";
            Content[0].data.name = "testing";*/
        }
    }
}
