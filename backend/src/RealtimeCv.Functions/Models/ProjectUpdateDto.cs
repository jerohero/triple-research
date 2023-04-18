using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class ProjectUpdateDto
{
    [JsonRequired]
    public int Id { get; set; }
    
    [JsonRequired]
    public string Name { get; set; }

    public ProjectUpdateDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
