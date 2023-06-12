using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Functions.Validators;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Functions.Services;

/// <summary>
/// Service for VisionSet related endpoints.
/// </summary>
public class VisionSetService : IVisionSetService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<VisionSetService> _logger;
    private readonly IVisionSetRepository _visionSetRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITrainedModelRepository _trainedModelRepository;

    public VisionSetService(
        ILoggerAdapter<VisionSetService> logger,
        IMapper mapper,
        IVisionSetRepository visionSetRepository,
        IProjectRepository projectRepository,
        ITrainedModelRepository trainedModelRepository
    )
    {
        _mapper = mapper;
        _logger = logger;
        _visionSetRepository = visionSetRepository;
        _projectRepository = projectRepository;
        _trainedModelRepository = trainedModelRepository;
    }

    public async Task<Result<VisionSetDto>> GetVisionSetById(int visionSetId)
    {
        var spec = new VisionSetWithTrainedModelSpec(visionSetId);
        var visionSet = await _visionSetRepository.SingleOrDefaultAsync(spec, CancellationToken.None);

        return visionSet is null
          ? Result<VisionSetDto>.NotFound()
          : new Result<VisionSetDto>(_mapper.Map<VisionSetDto>(visionSet));
    }

    public async Task<Result<List<VisionSetDto>>> GetVisionSetsByProject(int projectId)
    {
        var visionSets = await _visionSetRepository.ListAsync();

        return new Result<List<VisionSetDto>>(_mapper.Map<List<VisionSetDto>>(visionSets));
    }

    public async Task<Result<VisionSetDto>> CreateVisionSet(VisionSetCreateDto? createDto)
    {
        var validationResult = await new VisionSetCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<VisionSetDto>.Invalid(validationResult.AsErrors());
        }

        var project = await _projectRepository.GetByIdAsync(createDto.ProjectId);
        var trainedModel = await _trainedModelRepository.GetByIdAsync(createDto.TrainedModelId);

        if (project is null || trainedModel is null)
        {
            return Result<VisionSetDto>.NotFound();
        }
        
        var mappedVisionSet = _mapper.Map<VisionSet>(createDto);

        var visionSet = await _visionSetRepository.AddAsync(mappedVisionSet);

        return new Result<VisionSetDto>(_mapper.Map<VisionSetDto>(visionSet));
    }

    public async Task<Result<VisionSetDto>> UpdateVisionSet(VisionSetUpdateDto? updateDto)
    {
        var validationResult = await new VisionSetUpdateDtoValidator().ValidateAsync(updateDto!);

        if (updateDto is null || validationResult.Errors.Any())
        {
            return Result<VisionSetDto>.Invalid(validationResult.AsErrors());
        }

        var visionSet = await _visionSetRepository.GetByIdAsync(updateDto.Id);
        var trainedModel = await _trainedModelRepository.GetByIdAsync(updateDto.TrainedModelId);

        if (visionSet is null || trainedModel is null)
        {
            return Result<VisionSetDto>.NotFound();
        }

        visionSet = _mapper.Map(updateDto, visionSet);

        await _visionSetRepository.UpdateAsync(visionSet);

        return new Result<VisionSetDto>(_mapper.Map<VisionSetDto>(visionSet));
    }

    public async Task<Result> DeleteVisionSet(int visionSetId)
    {
        var visionSet = await _visionSetRepository.GetByIdAsync(visionSetId);

        if (visionSet is null)
        {
            return Result.NotFound();
        }

        await _visionSetRepository.DeleteAsync(visionSet);

        return Result.Success();
    }
}
