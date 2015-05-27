using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSearchDocDB.Models
{
    public class ImageQuerySaveModel
    {
        public int total { get; set; }
        public int limit { get; set; }
        public int count { get; set; }
        public IEnumerable<ImageQueryItemMode> items { get; set; }
        public DateTime Cached { get; set; }
        public int hit { get; set; }
        public string query { get; set; }
        public string id { get; set; }
    }

    public class ImageQueryItemMode
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string ID { get; set; }
    }

    public class QueryInfo
    {
        public string query { get; set; }
        public int hit { get; set; }
        public int total { get; set; }
    }
}