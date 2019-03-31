using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLogging
{
    public class Message : TableEntity
    {
        private const string MessagePartionKey = "LogEntry";
        private const string DateFormat = "yyyyMMdd ; HH:mm:ss:fffffff";
        private const string RowKeyFormat = "{0} - {1}";

        public string LogMessageType { get; set; }
        public string LogMessage { get; set; }

        public Message()
        {
        }

        public Message(string logMessageType, string logMessage)
        {
            PartitionKey = MessagePartionKey;
            string date = DateTime.Now.ToUniversalTime().ToString(DateFormat);
            RowKey = string.Format(RowKeyFormat, date, Guid.NewGuid().ToString());
            LogMessage = logMessage;
            LogMessageType = logMessageType;
        }
    }
}
