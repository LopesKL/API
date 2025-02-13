using Azure.Storage.Blobs.Models;

namespace API.Application.Dto
{
    public class BlobDto
    {
        public System.IO.Stream blob { get; set; }
        public BlobProperties prop { get; set; }
    }
}
