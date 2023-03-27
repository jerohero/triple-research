using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions;

public class AutomapperMaps: Profile
{
    public AutomapperMaps()
    {
        CreateMap<ProjectDto, Project>();
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectCreateDto, Project>();
        CreateMap<Project, ProjectCreateDto>();
    }
}
