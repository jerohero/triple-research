using Newtonsoft.Json;

namespace RealtimeCv.Core.Models.Dto;

public class ProjectCreateDto
{
    [JsonRequired]
    public string Name { get; set; }

    public ProjectCreateDto(string name)
    {
        Name = name;
    }
}
