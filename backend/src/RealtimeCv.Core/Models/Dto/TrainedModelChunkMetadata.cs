namespace RealtimeCv.Core.Models.Dto;

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
