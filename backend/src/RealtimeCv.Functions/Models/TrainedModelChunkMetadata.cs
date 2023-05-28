namespace RealtimeCv.Functions.Models;

public class TrainedModelChunkMetadata
{
    public string FileName { get; }
    
    public TrainedModelChunkMetadata(string fileName)
    {
        FileName = fileName;
    }
}
