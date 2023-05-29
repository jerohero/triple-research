using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class TrainedModelDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; } = "";
    
    [JsonRequired]
    public bool IsUploadFinished { get; set; }
    
    [JsonRequired]
    public int ProjectId { get; set; }

    public TrainedModelDto(string name, bool isUploadFinished, int projectId)
    {
        Name = name;
        IsUploadFinished = isUploadFinished;
        ProjectId = projectId;
    }
}
