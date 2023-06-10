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

public class VisionSetServiceTestsBase
{
    protected VisionSetService _service;
    protected AppDbContext _context;

    [SetUp]
    public void Setup()
    {
        var mocker = new AutoMocker();
        
        _context?.Database.EnsureDeleted();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var loggerMock = mocker.GetMock<ILoggerAdapter<VisionSetService>>();
        var blobMock = mocker.GetMock<IBlob>();
        
        var mapperConfig = new MapperConfiguration(cfg => 
            cfg.AddProfile(new AutomapperMaps())
        );
        var mapper = new Mapper(mapperConfig);

        _service = new VisionSetService(
            loggerMock.Object, mapper, new VisionSetRepository(_context),
            new ProjectRepository(_context), new TrainedModelRepository(_context)
        );
        
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    protected void SetupVisionSets(int count)
    {
        var fakeVisionSets = new List<VisionSet>();
        
        var fakeProject = new Project
        {
            Name = "project"
        };
        
        _context.Project.Add(fakeProject);
        _context.SaveChanges();

        var fakeTrainedModel = new TrainedModel
        {
            Name = "test.pt",
            IsUploadFinished = true,
            ProjectId = fakeProject.Id
        };
        
        _context.TrainedModel.Add(fakeTrainedModel);
        _context.SaveChanges();

        for (var i = 0; i < count; i++)
        {
            fakeVisionSets.Add(new VisionSet
            {
                Name = $"test{i}",
                ContainerImage = "test/image:latest",
                Sources = new List<string> { "rtsp://test.com" },
                ProjectId = fakeProject.Id,
                TrainedModelId = fakeTrainedModel.Id
            });
        }

        _context.VisionSet.AddRange(fakeVisionSets);
        _context.SaveChanges();
    }
}
