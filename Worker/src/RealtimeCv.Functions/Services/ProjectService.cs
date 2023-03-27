using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Controllers;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Services;

public class ProjectService : IProjectService
{
  private readonly IMapper _mapper;
  private readonly ILoggerAdapter<ProjectService> _logger;
  private readonly IProjectRepository _projectRepository;
  private readonly IVisionSetRepository _visionSetRepository;
  
  public ProjectService(
    ILoggerAdapter<ProjectService> logger,
    IMapper mapper,
    IProjectRepository projectRepository,
    IVisionSetRepository visionSetRepository
  )
  {
    _mapper = mapper;
    _logger = logger;
    _projectRepository = projectRepository;
    _visionSetRepository = visionSetRepository; 
  }
  
  public Task<ProjectDto> GetProject(int projectId)
  {
    throw new System.NotImplementedException();
  }

  public Task<List<ProjectDto>> GetProjects()
  {
    throw new System.NotImplementedException();
  }

  public async Task<ProjectDto> CreateProject(ProjectCreateDto projectCreateDto)
  {
    Project project = await _projectRepository.AddAsync(_mapper.Map<Project>(projectCreateDto));
    
    return _mapper.Map<ProjectDto>(project);
  }

  public async Task<ProjectDto> UpdateProject(ProjectDto projectDto)
  {
    await _projectRepository.UpdateAsync(_mapper.Map<Project>(projectDto));

    Project? project = await _projectRepository.GetByIdAsync(projectDto.Id);

    return _mapper.Map<ProjectDto>(project);
  }

  public Task DeleteProject(int projectId)
  {
    throw new System.NotImplementedException();
  }
}
