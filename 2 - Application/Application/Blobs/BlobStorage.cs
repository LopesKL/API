using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using API.Application.Dto;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Blobs
{
    class BlobStorage
    {
        private readonly string _storageConnection;
        private readonly string _storageConnectionContainer;
        private readonly IConfiguration _configuration;

        public BlobStorage(IConfiguration configuration) : base()
        {
            _configuration = configuration;
            _storageConnection = _configuration.GetConnectionString("AzureBlobStorage");
            _storageConnectionContainer = _configuration.GetConnectionString("AzureBlobStorageContainer");
        }

        [Serializable]
        public class UnauthorizedException : Exception
        {
            public UnauthorizedException() { }
            public UnauthorizedException(string message) : base(message) { }
            public UnauthorizedException(string message, System.Exception inner) : base(message, inner) { }
            protected UnauthorizedException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        private BlobContainerClient GetContainerClient()
        {
            return new BlobContainerClient(_storageConnection, _storageConnectionContainer);
        }

        public async Task UploadFile(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var container = GetContainerClient();
                await container.CreateIfNotExistsAsync();

                var blob = container.GetBlobClient(fileName);

                using (var stream = fileStream)
                {
                    var blobHeader = new BlobHttpHeaders() { ContentType = contentType };
                    await blob.UploadAsync(stream, blobHeader, cancellationToken: cancellationToken);
                }
            }
            catch (Azure.RequestFailedException e)
            {
                string message = "Request Failed Exception azure";
                if (e.ErrorCode == "AuthorizationFailure")
                {
                    message = "This request is not authorized to perform this operation";
                }

                throw new UnauthorizedException(message, e);
            }
        }

        public async Task RemoveFile(string filename, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient();
            var blob = container.GetBlobClient(filename);
            await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task<(System.IO.Stream, string)> GetFile(string filename, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient();
            var blob = container.GetBlobClient(filename);
            var properties = await blob.GetPropertiesAsync(cancellationToken: cancellationToken);
            return (await blob.OpenReadAsync(), properties.Value.ContentType);
        }

        public async Task<BlobDto> DownloadFile(string filename, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient();
            var blob = container.GetBlobClient(filename);
            var properties = await blob.GetPropertiesAsync(cancellationToken: cancellationToken);
            Stream blobStream = blob.OpenRead();

            return new BlobDto()
            {
                blob = blobStream,
                prop = properties
            };
        }
    }
}
