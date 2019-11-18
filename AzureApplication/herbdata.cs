using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureApplication
{
    public static class herbdata
    {
        [FunctionName("herbdata")]
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "herbdata/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            CloudStorageAccount sourceStorageAccount = new CloudStorageAccount(new StorageCredentials("ayurdatastorage", "YTNMML3QWTEeuniqotW6G0b0OpfgsJ5QFxOYnHVM3M0kwPjn1eNE2CBQcZjMEY1VsDvgwT095G+32IA6yJnhKg=="), true);
            CloudBlobClient sourceCloudBlobClient = sourceStorageAccount.CreateCloudBlobClient();

            var container = sourceCloudBlobClient.GetContainerReference("details");
            var uri = new Uri(sourceCloudBlobClient.BaseUri, $"/details/"+ name+".txt");
            // get the blob object and download to file
       
            var blobRef = await sourceCloudBlobClient.GetBlobReferenceFromServerAsync(uri);
            blobRef.FetchAttributes();
            long fileByteLength = blobRef.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            blobRef.DownloadToByteArray(fileContent, 0);
            return req.CreateResponse(HttpStatusCode.OK, fileContent);
        }
    }
}
