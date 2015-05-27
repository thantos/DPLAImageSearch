using ImageSearchDocDB.Models;
using ImageSearchDocDB.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ImageSearchDocDB.Controllers
{
    public class ImageController : ApiController
    {
        ImageSourceRepo repo = new ImageSourceRepo();

        [HttpGet]
        public async Task<ImageQuerySaveModel> Get(string query)
        {
            return await repo.Query(query, false);
        }

        public async Task<IEnumerable<QueryInfo>> GetQueryInfo()
        {
            return await repo.GetQueryInfo();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (repo != null)
                    repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
