using AzureLogging;
using AzureStorageTask_BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AzureStorageTask.Controllers
{
    [RoutePrefix("api/images")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ImageController : ApiController
    {
        private readonly StorageService service = new StorageService();
        private readonly Logging logging = new Logging();
        private readonly string destination = ConfigurationManager.
            AppSettings["DestinationFolder"];

        [HttpGet]
        [Route("{id}")]
        public async Task GetAsync(string id)
        {
            logging.StoreNewLogMessage(new Message("Info", $"Getting image with id {id}"));

            try
            {
                using (var fileStream = new FileStream($"{destination}{id}", FileMode.OpenOrCreate))
                {
                    await service.DownloadBlobFromStreamAsync(id, fileStream);
                    logging.StoreNewLogMessage(new Message("Info", $"Image with id {id} received"));
                }
            }
            catch (Exception ex)
            {
                logging.StoreNewLogMessage(new Message("Error", $"Exception: {ex.Message}"));
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task PostAsync([FromBody]string value)
        {
            logging.StoreNewLogMessage(new Message("Info", $"Posting image {value}"));

            try
            {
                using (var fileStream = new FileStream($"{destination}{value}", FileMode.Open))
                {
                    await service.UploadBlobFromStreamAsync(value, fileStream, false);
                    logging.StoreNewLogMessage(new Message("Info", $"Image {value} transmited"));
                }
            }
            catch(Exception ex)
            {
                logging.StoreNewLogMessage(new Message("Error", $"Exception: {ex.Message}"));
                throw;
            }
        }
    }
}
