using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Interfaces;

public interface IVisionSetService
{
    Task<Result<VisionSetDto>> GetVisionSetById(int visionSetId);
    Task<Result<List<VisionSetDto>>> GetVisionSetsByProject(int projectId);
    Task<Result<VisionSetDto>> CreateVisionSet(VisionSetCreateDto createDto);
    Task<Result<VisionSetDto>> UpdateVisionSet(VisionSetUpdateDto updateDto);
    Task<Result> DeleteVisionSet(int visionSetId);
}
