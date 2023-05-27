using System;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Blob;

public class Blob : IBlob
{
    private const string ConnStringName = "AzureWebJobsStorage";
    private readonly IConfiguration _configuration;
    private readonly ILoggerAdapter<Blob> _logger;
    
    public Blob(
        IConfiguration configuration,
        ILoggerAdapter<Blob> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public string GetBlobUri(string blobName, string containerName)
    {
        var connString = _configuration.GetConnectionString(ConnStringName);
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
}
