using Azure.Storage.Blobs;
using freelancer_hub_backend.Settings;
using Microsoft.Extensions.Options;

namespace freelancer_hub_backend.Services
{
    public class BlobStorageService
    {
        private readonly AzureStorageSettings _settings;

        public BlobStorageService(IOptions<AzureStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}