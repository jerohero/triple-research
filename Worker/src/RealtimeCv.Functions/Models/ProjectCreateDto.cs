using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class ProjectCreateDto
{
  [JsonRequired]
  public string Name { get; set; }

  public ProjectCreateDto(string name)
  {
    Name = name;
  }
}
