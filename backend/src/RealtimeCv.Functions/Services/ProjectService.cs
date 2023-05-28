using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;
using RealtimeCv.Functions.Validators;

namespace RealtimeCv.Functions.Services;

/// <summary>
/// Service for Project related endpoints.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<ProjectService> _logger;
    private readonly IProjectRepository _projectRepository;
    private readonly ITrainedModelRepository _trainedModelRepository;

    public ProjectService(
      ILoggerAdapter<ProjectService> logger,
      IMapper mapper,
      IProjectRepository projectRepository,
      ITrainedModelRepository trainedModelRepository
    )
    {
        _mapper = mapper;
        _logger = logger;
        _projectRepository = projectRepository;
        _trainedModelRepository = trainedModelRepository;
    }

    public async Task<Result<ProjectDto>> GetProjectById(int projectId)
    {
        var spec = new ProjectSpec(projectId);
        
        var project = await _projectRepository.SingleOrDefaultAsync(spec, CancellationToken.None);

        return project is null
          ? Result<ProjectDto>.NotFound()
          : new Result<ProjectDto>(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result<List<ProjectsDto>>> GetProjects()
    {
        var projects = await _projectRepository.ListAsync();

        return new Result<List<ProjectsDto>>(_mapper.Map<List<ProjectsDto>>(projects));
    }

    public async Task<Result<ProjectDto>> CreateProject(ProjectCreateDto? createDto)
    {
        // Don't want to use the dammit operator here, but the method requires a non-null value even though the validator will catch it
        var validationResult = await new ProjectCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<ProjectDto>.Invalid(validationResult.AsErrors());
        }

        var project = await _projectRepository.AddAsync(_mapper.Map<Project>(createDto));

        return new Result<ProjectDto>(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result<ProjectDto>> UpdateProject(ProjectUpdateDto? updateDto)
    {
        var validationResult = await new ProjectUpdateDtoValidator().ValidateAsync(updateDto!);

        if (updateDto is null || validationResult.Errors.Any())
        {
            return Result<ProjectDto>.Invalid(validationResult.AsErrors());
        }

        var project = await _projectRepository.GetByIdAsync(updateDto.Id);

        if (project is null)
        {
            return Result<ProjectDto>.NotFound();
        }

        project.UpdateName(updateDto.Name);

        await _projectRepository.UpdateAsync(project);

        return new Result<ProjectDto>(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result> DeleteProject(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            return Result.NotFound();
        }

        await _projectRepository.DeleteAsync(project);

        return Result.Success();
    }

    public async Task<Result> UploadTrainedModelChunk(Stream chunk, string? chunkName, int projectId)
    {
        if (chunkName is null)
        {
            return Result.Error("Chunk name is required");
        }
        
        var connString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        
        var blobName = $"{projectId}/{chunkName}";
        
        var blobServiceClient = new BlobServiceClient(connString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("trained-model");
        var blockBlobClient = blobContainerClient.GetBlockBlobClient(blobName);

        // Handle first chunk
        if (!await blockBlobClient.ExistsAsync())
        {
            var result = await CreateTrainedModel(projectId, blobName);
            
            if (result.Status != ResultStatus.Ok)
            {
                return result;
            }
        }

        var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
        await blockBlobClient.StageBlockAsync(blockId, chunk);

        var blockList = await blockBlobClient.GetBlockListAsync();
        var blockIds = blockList.Value.CommittedBlocks.Select(x => x.Name).ToList();
        blockIds.Add(blockId);

        await blockBlobClient.CommitBlockListAsync(blockIds);

        return Result.Success();
    }

    private async Task<Result> CreateTrainedModel(int projectId, string blobName)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
            
        if (project is null)
        {
            return Result.NotFound();
        }

        await _trainedModelRepository.AddAsync(new TrainedModel
        {
            ProjectId = projectId,
            Name = blobName,
        });

        return Result.Success();
    }
}
