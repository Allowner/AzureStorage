using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageTask_BLL
{
    public class StorageService
    {
        private readonly string connectionString;

        private readonly string containerName;

        private CloudQueue queue;

        public StorageService()
        {
            connectionString = ConfigurationManager.AppSettings["StorageAccount.ConnectionString"];

            containerName = "images";

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();

            queue = queueClient.GetQueueReference("queue");

            queue.CreateIfNotExists();
        }

        public async Task<CloudBlobContainer> GetCloudBlobContainerAsync()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            await cloudBlobContainer.CreateIfNotExistsAsync();

            return cloudBlobContainer;
        }

        public async Task<CloudBlockBlob> GetCloudBlockBlobAsync(string blobName)
        {
            CloudBlobContainer blobContainer = await this.GetCloudBlobContainerAsync();
            
            CloudBlockBlob cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);

            return cloudBlockBlob;
        }

        public async Task UploadBlobFromStreamAsync(string blobName, Stream stream, bool isBlured)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(blobName);
            
            await blob.UploadFromStreamAsync(stream);

            if (!isBlured)
            {
                CloudQueueMessage message = new CloudQueueMessage(blobName);

                queue.AddMessage(message);
            }
        }

        public async Task DownloadBlobFromStreamAsync(string blobName, Stream stream)
        {
            CloudBlockBlob blob = await GetCloudBlockBlobAsync(blobName);

            await blob.DownloadToStreamAsync(stream);
        }
    }
}
