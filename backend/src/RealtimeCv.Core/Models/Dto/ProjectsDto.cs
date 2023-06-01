using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

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
