using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace Core
{
    public class StorageRepository : IStorageRepository
    {
        private const string ContainerName = "storageservice";
        private readonly string _connectionString;

        public StorageRepository(string storageConnectionString)
        {
            _connectionString = storageConnectionString;
        }

        public async Task<string> SaveMessageToStorage<T>(T message)
        {
            var transactionId = Guid.NewGuid();
            var jsonFileName = $"queuemessages/{transactionId.ToString()}.json";
            var jsonText = JsonConvert.SerializeObject(message);

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerReferenece = blobClient.GetContainerReference(ContainerName);
            containerReferenece.CreateIfNotExists();

            var jsonFile = containerReferenece.GetBlockBlobReference(jsonFileName);
            await jsonFile.UploadTextAsync(jsonText);

            return jsonFileName;
        }

        public async Task<T> RestoreMessageFromStorage<T>(string jsonFileName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerReferenece = blobClient.GetContainerReference(ContainerName);
            containerReferenece.CreateIfNotExists();

            var jsonFile = containerReferenece.GetBlockBlobReference(jsonFileName);
            var message = await jsonFile.DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}