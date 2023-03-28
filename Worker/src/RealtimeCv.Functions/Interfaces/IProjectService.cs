using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectDto>> GetProject(int projectId);
    Task<Result<List<ProjectDto>>> GetProjects();
    Task<Result<ProjectDto>> CreateProject(ProjectCreateDto? createDto);
    Task<Result<ProjectDto>> UpdateProject(ProjectDto? updateDto);
    Task<Result> DeleteProject(int projectId);
}
