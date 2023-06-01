using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq.AutoMock;
using NUnit.Framework;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Functions.Config;
using RealtimeCv.Core.Functions.Services;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;

namespace RealtimeCv.UnitTests.Core.Functions.Services;

public class ProjectServiceTestsBase
{
    protected ProjectService _service;
    protected AppDbContext _context;

    [SetUp]
    public void Setup()
    {
        var mocker = new AutoMocker();
        
        _context?.Database.EnsureDeleted();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var loggerMock = mocker.GetMock<ILoggerAdapter<ProjectService>>();
        var blobMock = mocker.GetMock<IBlob>();
        
        var mapperConfig = new MapperConfiguration(cfg => 
            cfg.AddProfile(new AutomapperMaps())
        );
        var mapper = new Mapper(mapperConfig);

        _service = new ProjectService(
            loggerMock.Object, mapper, new ProjectRepository(_context),
            new TrainedModelRepository(_context), blobMock.Object
        );
        
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    protected void SetupProjects(int count)
    {
        var fakeProjects = new List<Project>();

        for (var i = 0; i < count; i++)
        {
            fakeProjects.Add(new Project
            {
                Name = $"Project{i+1}"
            });
        }
        
        _context.Project.AddRange(fakeProjects);
        _context.SaveChanges();
    }
}
