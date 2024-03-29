﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Newtonsoft.Json;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Functions.Validators;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Functions.Services;

/// <summary>
/// Service for Project related endpoints.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<ProjectService> _logger;
    private readonly IProjectRepository _projectRepository;
    private readonly ITrainedModelRepository _trainedModelRepository;
    private readonly IVisionSetRepository _visionSetRepository;
    private readonly IBlob _blob;

    public ProjectService(
      ILoggerAdapter<ProjectService> logger,
      IMapper mapper,
      IProjectRepository projectRepository,
      ITrainedModelRepository trainedModelRepository,
      IVisionSetRepository visionSetRepository,
      IBlob blob
    )
    {
        _mapper = mapper;
        _logger = logger;
        _projectRepository = projectRepository;
        _trainedModelRepository = trainedModelRepository;
        _visionSetRepository = visionSetRepository;
        _blob = blob;
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
        var spec = new ProjectsSpec();
        
        var projects = await _projectRepository.ListAsync(spec, CancellationToken.None);

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

        project = _mapper.Map(updateDto, project);

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

    public async Task<Result> UploadTrainedModelChunk(Stream chunk, string? fileName, int? size, int projectId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Error("Chunk name is required");
        }
        if (size is null)
        {
            return Result.Error("Chunk size is required");
        }
        
        var blobName = $"{projectId}/{fileName}";
        var blockBlobClient = _blob.GetBlockBlobClient(blobName, "trained-model");

        var blobExists = await blockBlobClient.ExistsAsync();
        TrainedModel? trainedModel;

        if (blobExists.Value)
        {
            var trainedModelSpec = new TrainedModelByNameSpec(blobName);
            trainedModel = await _trainedModelRepository.SingleOrDefaultAsync(trainedModelSpec, CancellationToken.None);

            if (trainedModel is not null && trainedModel.IsUploadFinished)
            {
                return Result.Conflict("A model already exists under this name");
            }
        }
        else
        {
            var result = await CreateTrainedModel(projectId, blobName);
            
            if (result.Status == ResultStatus.NotFound)
            {
                return Result.NotFound();
            }
            if (result.Status != ResultStatus.Ok)
            {
                return Result.Error(result.Errors.First());
            }

            trainedModel = result.Value;
        }

        await _blob.UploadBlockBlob(blockBlobClient, chunk);

        var isFinished = await _blob.IsBlockBlobUploadFinished(blockBlobClient, (int) size);

        if (!isFinished)
        {
            return Result.Success();
        }

        trainedModel!.IsUploadFinished = true;
        await _trainedModelRepository.UpdateAsync(trainedModel);

        return Result.Success();
    }
    
    public async Task<Result<List<TrainedModelDto>>> GetTrainedModels(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
            
        if (project is null)
        {
            return Result.NotFound();
        }
        
        var spec = new TrainedModelsByProject(projectId);
        var trainedModels = await _trainedModelRepository.ListAsync(spec, CancellationToken.None);

        return new Result<List<TrainedModelDto>>(_mapper.Map<List<TrainedModelDto>>(trainedModels));
    }

    private async Task<Result<TrainedModel>> CreateTrainedModel(int projectId, string blobName)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
            
        if (project is null)
        {
            return Result.NotFound();
        }
        
        TrainedModelByNameSpec trainedModelSpec = new(blobName);

        var existingTrainedModel = await _trainedModelRepository.FirstOrDefaultAsync(trainedModelSpec, CancellationToken.None);
        
        if (existingTrainedModel is not null)
        {
            return Result.Conflict("A model already exists under this name");
        }

        var trainedModel = await _trainedModelRepository.AddAsync(new TrainedModel
        {
            ProjectId = projectId,
            IsUploadFinished = false,
            Name = blobName,
        });

        return new Result<TrainedModel>(trainedModel);
    }
    
    public async Task<Result> DeleteTrainedModel(int trainedModelId)
    {
        var trainedModel = await _trainedModelRepository.GetByIdAsync(trainedModelId);

        if (trainedModel is null)
        {
            return Result.NotFound();
        }
        
        var visionSetsSpec = new VisionSetsByTrainedModel(trainedModelId);
        var visionSetsUsingTrainedModel = await _visionSetRepository.ListAsync(visionSetsSpec, CancellationToken.None);

        if (visionSetsUsingTrainedModel.Count > 0)
        {
            return Result.Conflict("This model is being used by one or more vision sets and can not be deleted.");
        }

        await _blob.DeleteBlob(trainedModel.Name, "trained-model");

        await _trainedModelRepository.DeleteAsync(trainedModel);

        return Result.Success();
    }
}
