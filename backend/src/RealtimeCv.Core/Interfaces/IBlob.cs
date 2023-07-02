using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;

namespace RealtimeCv.Core.Interfaces;

public interface IBlob
{
    string GetBlobUri(string blobName, string containerName);
    BlockBlobClient GetBlockBlobClient(string blobName, string containerName);
    Task UploadBlockBlob(BlockBlobClient blockBlobClient, Stream chunk);
    Task<bool> IsBlockBlobUploadFinished(BlockBlobClient blockBlobClient, int expectedSize);
    Task DeleteBlob(string blobName, string containerName);
}
