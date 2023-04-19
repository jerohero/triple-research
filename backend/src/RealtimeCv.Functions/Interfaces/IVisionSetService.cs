using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Interfaces;

public interface IVisionSetService
{
    Task<Result<VisionSetDto>> GetVisionSetById(int visionSetId);
    Task<Result<List<VisionSetDto>>> GetVisionSetsByProject(int projectId);
    Task<Result<VisionSetDto>> CreateVisionSet(VisionSetCreateDto? createDto);
    Task<Result<VisionSetDto>> UpdateVisionSet(VisionSetDto? updateDto);
    Task<Result> DeleteVisionSet(int visionSetId);
}
