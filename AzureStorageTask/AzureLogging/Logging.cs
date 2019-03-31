using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLogging
{
    public class Logging
    {
        private readonly CloudTable table;

        public Logging()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(
            ConfigurationManager.AppSettings["StorageAccount.ConnectionString"]);
            CloudTableClient client = account.CreateCloudTableClient();
            table = client.GetTableReference("table");
            table.CreateIfNotExists();
        }

        public void StoreNewLogMessage(Message logMessage)
        { 
            TableOperation insertOperation = TableOperation.Insert(logMessage);
            table.Execute(insertOperation);
        }
    }
}
