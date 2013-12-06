using Newtonsoft.Json;
using RedditReader.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RedditReader
{
    class RedditAPI
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

        public RedditAPI()
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Listing.RootObject GetListing(string subreddit)
        {
            HttpResponseMessage response = client.GetAsync(_API_URL + subreddit + ".json").Result;
            return JsonConvert.DeserializeObject<Listing.RootObject>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
