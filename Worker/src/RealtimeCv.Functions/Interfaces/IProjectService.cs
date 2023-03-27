using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Interfaces;

public interface IProjectService
{
  Task<ProjectDto> GetProject(int projectId);
  Task<List<ProjectDto>> GetProjects();
  Task<ProjectDto> CreateProject(ProjectCreateDto projectCreateDto);
  Task<ProjectDto> UpdateProject(ProjectDto projectDto);
  Task DeleteProject(int projectId);
}
