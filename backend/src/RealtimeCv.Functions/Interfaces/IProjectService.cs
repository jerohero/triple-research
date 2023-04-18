using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectDto>> GetProjectById(int projectId);
    Task<Result<List<ProjectsDto>>> GetProjects();
    Task<Result<ProjectDto>> CreateProject(ProjectCreateDto? createDto);
    Task<Result<ProjectDto>> UpdateProject(ProjectUpdateDto? updateDto);
    Task<Result> DeleteProject(int projectId);
}
