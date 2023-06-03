using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Blob;

public class Blob : IBlob
{
    private const string ConnStringName = "AzureWebJobsStorage";
    private readonly ILoggerAdapter<Blob> _logger;
    
    public Blob(
        ILoggerAdapter<Blob> logger
    )
    {
        _logger = logger;
    }
    
    public string GetBlobUri(string blobName, string containerName)
    {
        var connString = Environment.GetEnvironmentVariable(ConnStringName);
        var accountName = Environment.GetEnvironmentVariable("StorageAccountName");
        var accountKey = Environment.GetEnvironmentVariable("StorageAccountKey");
        
        var container = new BlobContainerClient(connString, containerName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };
        
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        var credential = new StorageSharedKeyCredential(accountName, accountKey);
        var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

        var blobUri = $"{container.Uri}/{blobName}?{sasToken}";

        return blobUri;
    }

    public BlockBlobClient GetBlockBlobClient(string blobName, string containerName)
    {
        var connString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var blobServiceClient = new BlobServiceClient(connString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        
        return blobContainerClient.GetBlockBlobClient(blobName);
    }

    public async Task UploadBlockBlob(BlockBlobClient blockBlobClient, Stream chunk)
    {
        var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        await blockBlobClient.StageBlockAsync(blockId, chunk);

        var blockList = await blockBlobClient.GetBlockListAsync();
        var blockIds = blockList.Value.CommittedBlocks.Select(x => x.Name).ToList();
        blockIds.Add(blockId);

        await blockBlobClient.CommitBlockListAsync(blockIds);
    }
    
    public async Task<bool> IsBlockBlobUploadFinished(BlockBlobClient blockBlobClient, int expectedSize)
    {
        var length = (await blockBlobClient.GetPropertiesAsync()).Value.ContentLength;
        
        return length >= expectedSize;
    }
    
    
}
