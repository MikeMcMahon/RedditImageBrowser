﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditImageBrowser.Json
{
    public class SubredditSearch
    {
        public class ByName : JsonApi
        {
            public List<string> names { get; set; }
        }
    }
}
