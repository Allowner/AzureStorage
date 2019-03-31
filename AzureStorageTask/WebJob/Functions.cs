using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using AzureStorageTask_BLL;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebJob
{
    public class Functions
    {
        public static StorageService service = new StorageService();

        public static async Task ProcessQueueMessageAsync([QueueTrigger("queue")] string message, TextWriter log)
        {
            Bitmap bitmap = null;
            using (var fileStream = new FileStream(message, FileMode.OpenOrCreate))
            {
                await service.DownloadBlobFromStreamAsync(message, fileStream);
                bitmap = new Bitmap(fileStream);
            }

            StackBlur.StackBlur.Process(bitmap, 10);
            bitmap.Save(message);

            using (var fileStream = new FileStream(message, FileMode.Open))
            {
                await service.UploadBlobFromStreamAsync(message, fileStream, true);
            }
        }
    }
}
