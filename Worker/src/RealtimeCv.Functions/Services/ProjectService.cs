using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using FluentValidation.Results;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;
using RealtimeCv.Functions.Validators;

namespace RealtimeCv.Functions.Services;

public class ProjectService : IProjectService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<ProjectService> _logger;
    private readonly IProjectRepository _projectRepository;

    public ProjectService(
      ILoggerAdapter<ProjectService> logger,
      IMapper mapper,
      IProjectRepository projectRepository
    )
    {
        _mapper = mapper;
        _logger = logger;
        _projectRepository = projectRepository;
    }

    public async Task<Result<ProjectDto>> GetProject(int projectId)
    {
        Project? project = await _projectRepository.GetByIdAsync(projectId);

        return project is null
          ? Result<ProjectDto>.NotFound($"Project with id {projectId} does not exist")
          : new Result<ProjectDto>(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result<List<ProjectDto>>> GetProjects()
    {
        List<Project> projects = await _projectRepository.ListAsync();

        return new Result<List<ProjectDto>>(_mapper.Map<List<ProjectDto>>(projects));
    }

    public async Task<Result<ProjectDto>> CreateProject(ProjectCreateDto? createDto)
    {
        // Don't want to use the dammit operator here, but the method requires a non-null value even though the validator will catch it
        ValidationResult validationResult = await new ProjectCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<ProjectDto>.Invalid(validationResult.AsErrors());
        }

        Project project = await _projectRepository.AddAsync(_mapper.Map<Project>(createDto));

        return new Result<ProjectDto>(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result<ProjectDto>> UpdateProject(ProjectDto? updateDto)
    {
        ValidationResult validationResult = await new ProjectDtoValidator().ValidateAsync(updateDto!);

        if (updateDto is null || validationResult.Errors.Any())
        {
            return Result<ProjectDto>.Invalid(validationResult.AsErrors());
        }

        Project? project = await _projectRepository.GetByIdAsync(updateDto.Id);

        if (project is null)
        {
            return Result<ProjectDto>.Error("Project not found");
        }

        project.UpdateName(updateDto.Name);

        await _projectRepository.UpdateAsync(project);

        return new Result<ProjectDto>(updateDto);
    }

    public async Task<Result> DeleteProject(int projectId)
    {
        Project? project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return Result.Error("Project not found");
        }

        await _projectRepository.DeleteAsync(project);

        return Result.Success();
    }
}
