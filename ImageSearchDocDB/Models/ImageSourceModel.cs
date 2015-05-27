using ImageSearchDocDB.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSearchDocDB.Models
{
    public class SearchModel
    {
        public int count { get; set; }
        public int start { get; set; }
        public int limit { get; set; }
        public List<Doc> Docs { get; set; }
    }

    public class Doc
    {
        public Provider Provider { get; set; }
        public string isShownAt { get; set; }
        public string id { get; set; }
        public string _id { get; set; }
        public string @object { get; set; }
        public Resource sourceResource { get; set; }
        public Record originalRecord { get; set; }
    }

    public class Resource
    {
        [JsonConverter(typeof(SingleValueArrayConverter))]
        public List<string> title { get; set; }
        public List<Subject> subject { get; set; }
        [JsonConverter(typeof(SingleValueArrayConverter))]
        public List<string> type { get; set; }
    }

    public class Record
    {
        public string link { get; set; }
        public string image_link { get; set; }
        public List<string> tags { get; set; }
        public string tmp_item_link { get; set; }
        public string tmp_high_res_link { get; set; }
    }

    public class Subject
    {
        public string Name { get; set; }
    }

    public class Provider
    {
        public string Name { get; set; }
    }
}