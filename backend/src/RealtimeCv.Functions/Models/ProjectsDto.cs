using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class ProjectsDto
{
    [JsonRequired]
    public int Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }

    public ProjectsDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
