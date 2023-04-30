using AutoMapper;
using RealtimeCv.Functions.Models;
using RealtimeCv.Infrastructure.Entities;

namespace RealtimeCv.Functions;

public class AutomapperMaps : Profile
{
    public AutomapperMaps()
    {
        CreateMap<ProjectDto, Project>();
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectCreateDto, Project>();
        CreateMap<Project, ProjectCreateDto>();
    }
}
