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
            SubredditSearch.ByName names = null;

            if (!RedditFromPost<SubredditSearch.ByName>(_API_URL + "/api/search_reddit_names.json", postData, out names))
                return false; // TODO - maybe a meaningful message should be dispatched? like hey, your fucking network bro

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
            SubredditDetail details = null;
            RedditFromGet<SubredditDetail>(_API_URL + name + "/about.json", out details);
            return details;
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

        #region Subreddit Listings
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
            bool success = false;
            
            if (!fullname.Equals(""))
                next = GetListingDirection(forwards, fullname);

            int i = 0;
            do {
                if (listing == null)
                    success = RedditFromGet<Listing>(_API_URL + subreddit + ".json" + next, out listing);
                else
                    success = RedditFromGet(_API_URL + subreddit + ".json" + next, out tmpListing);

                if (success) {
                    if (tmpListing != null) {
                        foreach (var child in tmpListing.data.children) {
                            listing.data.children.Add(child);
                        }
                    }

                    GetNextListingFullname(forwards, listing, ref next);
                }

                if (next == null || next.Equals(""))
                    break;

                next = GetListingDirection(forwards, next);

                if (++i >= pages)
                    break;
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

        /// <summary>
        /// Gets the next listing item based on the direction of the scroll
        /// </summary>
        /// <param name="forwards"></param>
        /// <param name="listing"></param>
        /// <param name="next"></param>
        private void GetNextListingFullname(bool forwards, Listing listing, ref string next)
        {
            if (forwards)
                next = listing.data.after;
            else
                next = listing.data.before;
        }

        /// <summary>
        /// Based on the scroll direction generate a key=>value param suitable to be slapped onto a URL
        /// </summary>
        /// <param name="forwards"></param>
        /// <param name="fullname"></param>
        /// <returns></returns>
        private string GetListingDirection(bool forwards, string fullname)
        {
            string direction = "";

            if (forwards & fullname != null)
                direction = "?after=" + fullname;
            else if (!forwards & fullname != null)
                direction = "?before=" + fullname;

            return direction;
        }
        #endregion

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

        #region Actually Querying the API 

        /// <summary>
        /// Returns a reddit deserialized object from a post method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool RedditFromPost<T>(string url, Dictionary<string, string> postData, out T result)
        {
            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(postData);
            using (HttpResponseMessage response = client.PostAsync(url,httpContent).Result) {

                if (!response.IsSuccessStatusCode) {
                    result = (T)new Object();
                    return false;
                }

                string content = response.Content.ReadAsStringAsync().Result;
                result = Deserialize<T>(content);
            }

            return true;
        }

        /// <summary>
        /// Returns a reddit deseralized object from a get method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool RedditFromGet<T>(string url, out T result)
        {
            using (HttpResponseMessage response = client.GetAsync(url).Result) {

                if (!response.IsSuccessStatusCode) {
                    result = (T)new Object();
                    return false;
                }

                string content = response.Content.ReadAsStringAsync().Result;
                result = Deserialize<T>(content);
            }

            return true;
        }

        #endregion

        #region Json Related
        /// <summary>
        /// Deserializes to an object using our converters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        private T Deserialize<T>(string content)
        {
            // TODO - if received JSON is bad... CATCH IT
            return JsonConvert.DeserializeObject<T>(content, converters);
        }

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
