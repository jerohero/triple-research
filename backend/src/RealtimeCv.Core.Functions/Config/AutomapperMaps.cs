using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Functions.Config;

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
        CreateMap<Session, SessionDto>()
            .ForMember(dest => dest.Status, opt => opt.Ignore());
        CreateMap<SessionStartDto, Session>();
        
        CreateMap<TrainedModel, TrainedModelDto>();
    }
}
