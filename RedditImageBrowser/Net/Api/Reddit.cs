using Newtonsoft.Json;
using RedditImageBrowser.Common;
using RedditImageBrowser.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Net.Api
{
    class Reddit
    {
        private const string _AUTHORIZATION_URL = @"https://ssl.reddit.com/api/v1/authorize";
        private const string _ACCESS_TOKEN_URL = @"https://ssl.reddit.com/api/v1/access_token";

        /// <summary>
        /// Only used after authorization
        /// </summary>
        private const string _OAUTH_API_URL = @"https://oauth.reddit.com";
        
        /// <summary>
        /// Use this API URL until we are authenticated into the system
        /// </summary>
        private const string _API_URL = @"http://www.reddit.com/";

        private HttpClient client = new HttpClient();

        private JsonConverter[] converters = { new NullBooleans() };

        public Reddit()
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Is the subreddit valid?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValidSubreddit(string name)
        {
            string subreddit = ApiFormatFromUrl(name);
            Dictionary<string, string> postData = new Dictionary<string,string>();
            postData.Add("query", subreddit);
            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = client.PostAsync(_API_URL + "/api/search_reddit_names.json", httpContent).Result;
            string content = response.Content.ReadAsStringAsync().Result;
            SubredditSearch.ByName names = Deserialize<SubredditSearch.ByName>(content);

            foreach (string subredditName in names.names) {
                if (subredditName.ToLower().Equals(subreddit.ToLower())) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the details for a given subreddit
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SubredditDetail SubredditDetails(string name)
        {
            HttpResponseMessage response = client.GetAsync(_API_URL + name + "/about.json").Result;
            string content = response.Content.ReadAsStringAsync().Result;
            return Deserialize<SubredditDetail>(content);
        }

        /// <summary>
        /// Transforms a subreddit from url format /r/[subreddit] to api format [subreddit]
        /// </summary>
        /// <param name="subreddit"></param>
        /// <returns></returns>
        public string ApiFormatFromUrl(string subreddit)
        {
            return (subreddit.StartsWith("/r/")) ? subreddit.Substring(3) : subreddit;
        }


        /// <summary>
        /// Deserializes to an object using our converters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        private T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content, converters);
        }

        /// <summary>
        /// Gets the listing
        /// </summary>
        /// <param name="subreddit">the subreddit to search</param>
        /// <param name="pages">the number of pages to return</param>
        /// <param name="forwards">true = forwards, false = backwards</param>
        /// <param name="fullname">if searching from a midpoint... the fullname of the item</param>
        /// <returns>The listing from subreddit</returns>
        public Listing GetListing(string subreddit, int pages = 1, bool forwards = true, string fullname = "")
        {
            Listing listing = null;
            Listing tmpListing = null;
            string next = "";
            
            if (!fullname.Equals(""))
                next = GetListingDirection(forwards, fullname);

            int i = 0;
            do {
                using (HttpResponseMessage response = client.GetAsync(_API_URL + subreddit + ".json" + next).Result) {
                    string content = response.Content.ReadAsStringAsync().Result;

                    if (listing == null) {
                        listing = Deserialize<Listing>(content);

                        next = GetNextListingFullname(forwards, listing, next);

                    } else {
                        tmpListing = Deserialize<Listing>(content);

                        foreach (var child in tmpListing.data.children) {
                            listing.data.children.Add(child);
                        }

                        next = GetNextListingFullname(forwards, tmpListing, next);
                    }

                    if (next == null || next.Equals(""))
                        break;

                    next = GetListingDirection(forwards, next);

                    if (++i >= pages)
                        break;
                }
            } while (true);

            if (tmpListing != null) {
                listing.data.after = tmpListing.data.after;
                listing.data.before = tmpListing.data.before;
            }

            ScrubListing(listing);
            return listing;
        }

        /// <summary>
        /// Removes entries from the listing we cannot process yet or will never process (like stickies and non image having links)
        /// </summary>
        /// <param name="listing"></param>
        private void ScrubListing(Listing listing)
        {
            Uri uri = null;
            IList<Listing.Child> remove = new List<Listing.Child>();
            bool valid = false;

            foreach (Listing.Child child in listing.data.children) {
                { // supported thumbnails
                    try {
                        uri = new Uri(child.data.thumbnail);
                    } catch (UriFormatException e) {
                        remove.Add(child);
                        continue;
                    }
                }

                { // supported images
                    try {
                        uri = new Uri(child.data.url);
                    } catch (UriFormatException e) {
                        remove.Add(child);
                        continue;
                    }

                    valid = false;
                    foreach (var format in Config.supporfted_file_formats) {
                        if (uri.AbsolutePath.ToLower().EndsWith(format))
                            valid = true;
                    }

                    if (!valid)
                        remove.Add(child);
                }
            }

            foreach (Listing.Child bad in remove)
                listing.data.children.Remove(bad);
        }

        private static string GetNextListingFullname(bool forwards, Listing listing, string next)
        {
            if (forwards)
                next = listing.data.after;
            else
                next = listing.data.before;
            return next;
        }

        private string GetListingDirection(bool forwards, string fullname)
        {
            string direction = "";

            if (forwards & fullname != null)
                direction = "?after=" + fullname;
            else if (!forwards & fullname != null)
                direction = "?before=" + fullname;

            return direction;
        }

        /// <summary>
        /// Logs in via the standard login api call
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            return true;
        }

        #region JsonConverters
        private class NullBooleans : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (typeof(bool) == objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null)
                    return false;
                return reader.Value;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
