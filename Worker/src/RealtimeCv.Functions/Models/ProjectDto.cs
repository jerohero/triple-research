using Newtonsoft.Json;

namespace RealtimeCv.Functions.Models;

public class ProjectDto
{
  [JsonRequired]
  public int Id { get; set; }
  
  [JsonRequired]
  public string Name { get; set; }
  
  public ProjectDto(int id, string name)
  {
    Id = id;
    Name = name;
  }
}
