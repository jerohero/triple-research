using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;
using RealtimeCv.Functions.Validators;

namespace RealtimeCv.Functions.Services;

/// <summary>
/// Service for VisionSet related endpoints.
/// </summary>
public class VisionSetService : IVisionSetService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<VisionSetService> _logger;
    private readonly IVisionSetRepository _visionSetRepository;

    public VisionSetService(
      ILoggerAdapter<VisionSetService> logger,
      IMapper mapper,
      IVisionSetRepository visionSetRepository
    )
    {
        _mapper = mapper;
        _logger = logger;
        _visionSetRepository = visionSetRepository;
    }

    public async Task<Result<VisionSetDto>> GetVisionSetById(int visionSetId)
    {
        var visionSet = await _visionSetRepository.GetByIdAsync(visionSetId);

        return visionSet is null
          ? Result<VisionSetDto>.NotFound()
          : new Result<VisionSetDto>(_mapper.Map<VisionSetDto>(visionSet));
    }

    public async Task<Result<List<VisionSetDto>>> GetVisionSets()
    {
        var visionSets = await _visionSetRepository.ListAsync();

        return new Result<List<VisionSetDto>>(_mapper.Map<List<VisionSetDto>>(visionSets));
    }

    public async Task<Result<VisionSetDto>> CreateVisionSet(VisionSetCreateDto? createDto)
    {
        // Don't want to use the dammit operator here, but the method requires a non-null value even though the validator will catch it
        var validationResult = await new VisionSetCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<VisionSetDto>.Invalid(validationResult.AsErrors());
        }

        var visionSet = await _visionSetRepository.AddAsync(_mapper.Map<VisionSet>(createDto));

        return new Result<VisionSetDto>(_mapper.Map<VisionSetDto>(visionSet));
    }

    public async Task<Result<VisionSetDto>> UpdateVisionSet(VisionSetDto? updateDto)
    {
        var validationResult = await new VisionSetDtoValidator().ValidateAsync(updateDto!);

        if (updateDto is null || validationResult.Errors.Any())
        {
            return Result<VisionSetDto>.Invalid(validationResult.AsErrors());
        }

        var visionSet = await _visionSetRepository.GetByIdAsync(updateDto.Id);

        if (visionSet is null)
        {
            return Result<VisionSetDto>.NotFound();
        }

        visionSet.UpdateName(updateDto.Name);

        await _visionSetRepository.UpdateAsync(visionSet);

        return new Result<VisionSetDto>(updateDto);
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
