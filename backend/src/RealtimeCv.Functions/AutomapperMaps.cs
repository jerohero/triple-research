using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions;

public class AutomapperMaps : Profile
{
    public AutomapperMaps()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<Project, ProjectsDto>();
        CreateMap<ProjectCreateDto, Project>();
        CreateMap<ProjectUpdateDto, Project>();

        CreateMap<VisionSetDto, VisionSet>();
        CreateMap<VisionSet, VisionSetDto>();
        CreateMap<VisionSetCreateDto, VisionSet>();
        CreateMap<VisionSetUpdateDto, VisionSet>();
        
        CreateMap<SessionDto, Session>();
        CreateMap<Session, SessionDto>();
        CreateMap<SessionCreateDto, Session>();
        
        CreateMap<TrainedModel, TrainedModelDto>();
    }
}
