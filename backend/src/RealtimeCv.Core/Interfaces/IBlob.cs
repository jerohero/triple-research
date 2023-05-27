namespace RealtimeCv.Core.Interfaces;

public interface IBlob
{
    string GetBlobUri(string blobName, string containerName);
}
