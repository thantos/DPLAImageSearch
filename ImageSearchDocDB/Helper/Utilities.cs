using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace ImageSearchDocDB.Helper
{
    public static class Utilities
    {
        public static async Task<Database> SetupDatabase(DocumentClient client, string name)
        {
            return client.CreateDatabaseQuery().Where(d => d.Id == name).AsEnumerable().FirstOrDefault() ??
                await client.CreateDatabaseAsync(new Database() { Id = name });
        }

        public static async Task<DocumentCollection> SetupCollection(DocumentClient client, Database db, string name)
        {
            return client.CreateDocumentCollectionQuery(db.SelfLink).Where(c => c.Id == name).AsEnumerable().FirstOrDefault() ??
                await client.CreateDocumentCollectionAsync(db.CollectionsLink, new DocumentCollection() { Id = name });
        }

        public static async Task CleanUpDatabase(DocumentClient client, string DBName)
        {
            var db = await Utilities.SetupDatabase(client, DBName);
            await client.DeleteDatabaseAsync(db.SelfLink);
        }
    }
}