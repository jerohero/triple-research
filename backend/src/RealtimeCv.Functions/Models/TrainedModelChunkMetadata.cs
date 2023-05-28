namespace RealtimeCv.Functions.Models;

public class TrainedModelChunkMetadata
{
    public string Name { get; }
    public int Size { get; }
    
    public TrainedModelChunkMetadata(string name, int size)
    {
        Name = name;
        Size = size;
    }
}
