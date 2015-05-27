using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System.Net;
using ImageSearchDocDB.Helper;
using ImageSearchDocDB.Models;
using System.Configuration;

namespace ImageSearchDocDB.Repo
{
    public class ImageSourceRepo : IDisposable
    {
        private DocumentClient client = new DocumentClient(new Uri(EndpointUrl), AuthKey);

        private static string DBName = "ImageDB";
        private static string CollName = "ImageCollection";

        private static string APIRootUrl = "http://api.dp.la/v2/";
        private static string APIKey = ConfigurationManager.AppSettings["DPLKey"] ;
        private static string APISearchRoute = "items?q={0}&api_key={1}&page={2}&page_size=100";
        private static string APISearchAddType = "&sourceResource.type={3}";

        private static string EndpointUrl = ConfigurationManager.AppSettings["AzureEndpoint"];
        private static string AuthKey = ConfigurationManager.AppSettings["AzureAuthKey"];

        public async Task<ImageQuerySaveModel> Query(string query, bool cleanup = true)
        {
            var db = await Utilities.SetupDatabase(client, DBName);
            var coll = await Utilities.SetupCollection(client, db, CollName);

            var model = GetImageQuerySaveModel(coll.SelfLink, query);
            if (model != null)
            {
                model.hit++;
                await UpdateImageQuerySaveModel(coll.SelfLink, model);
            }
            else
            {
                var res = await DoSearch(query, 1, "image");

                var saveModel = new ImageQuerySaveModel()
                {
                    count = res.Docs.Count,
                    Cached = DateTime.Now,
                    limit = res.limit,
                    total = res.count,
                    items = res.Docs.Select(doc => new ImageQueryItemMode() { ID = doc.id, ImageUrl = doc.@object, Title = doc.sourceResource.title.FirstOrDefault() }),
                    hit = 1,
                    query = query
                };

                await this.AddImageQuerySaveModel(coll.SelfLink, saveModel);
                model = saveModel;
            }

            if (cleanup)
                await CleanUp(client);
            return model;
        }

        public static async Task CleanUp(DocumentClient client)
        {
            await Utilities.CleanUpDatabase(client, DBName);
        }

        public static async Task<SearchModel> DoSearch(string query, int page = 1, string type = null)
        {
            using (var cli = new WebClient())
            {
                var res = cli.DownloadString(new Uri(GetAPISearchUrl(query, page, type)));
                return await JsonConvert.DeserializeObjectAsync<SearchModel>(res);
            }
        }

        public static string GetAPISearchUrl(string query, int page, string type = null)
        {
            return string.Format(APIRootUrl + APISearchRoute + (!string.IsNullOrEmpty(type) ? APISearchAddType : string.Empty), query, APIKey, page, type);
        }

        public ImageQuerySaveModel GetImageQuerySaveModel(string collId, string query)
        {
            return client.CreateDocumentQuery<ImageQuerySaveModel>(collId).Where(i => i.query == query).ToList().FirstOrDefault();
        }

        public async Task AddImageQuerySaveModel(string collId, ImageQuerySaveModel saveModel)
        {
            await client.CreateDocumentAsync(collId, saveModel);
        }

        public async Task<Document> UpdateImageQuerySaveModel(string collId, ImageQuerySaveModel saveModel)
        {
            var doc = client.CreateDocumentQuery(collId).Where(d => d.Id == saveModel.id).ToList().FirstOrDefault();
            return (await client.ReplaceDocumentAsync(doc.SelfLink, saveModel)).Resource;
        }

        public async Task<IEnumerable<QueryInfo>> GetQueryInfo()
        {
            var db = await Utilities.SetupDatabase(client, DBName);
            var coll =await  Utilities.SetupCollection(client, db, CollName);
            return client.CreateDocumentQuery<QueryInfo>(coll.SelfLink,
                @"SELECT f.query, f.hit, f.total
FROM calls f").ToList();
        }

        public void Dispose()
        {
            if (client != null)
                client.Dispose();
        }
    }
}
