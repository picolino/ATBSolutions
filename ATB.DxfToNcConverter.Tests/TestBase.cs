using System.IO;
using ATB.DxfToNcConverter.Tests.Fakes;
using ATB.DxfToNcConverter.Tests.UnitTests.DSL;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests
{
    public abstract class TestBase
    {
        protected Builder GiveMe { get; private set; }
        protected ConfigurationServiceStub ConfigurationServiceStub { get; private set; }
        protected DxfServiceStub DxfServiceStub { get; private set; }
        protected FileSystemServiceStub FileSystemServiceStub { get; private set; }
        
        [SetUp]
        protected virtual void Setup()
        {
            GiveMe = new Builder();
            ConfigurationServiceStub = new ConfigurationServiceStub();
            DxfServiceStub = new DxfServiceStub();
            FileSystemServiceStub = new FileSystemServiceStub();
            
            SetupEcs();
        }

        private void SetupEcs()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);

            SetupWorld(world);
            SetupSystems(systems);

            InjectDependencies(systems);
            
            BeforeInitializeSystems(systems);
            
            systems.Init();
        }

        private void InjectDependencies(EcsSystems systems)
        {
            systems.Inject(ConfigurationServiceStub)
                   .Inject(DxfServiceStub)
                   .Inject(FileSystemServiceStub);
        }
        
        protected virtual void SetupWorld(EcsWorld world)
        {
        }

        protected abstract void SetupSystems(EcsSystems systems);

        protected virtual void BeforeInitializeSystems(EcsSystems systems)
        {
        }

        protected string GetTestFileContent(string relativePath)
        {
            return File.ReadAllText(TestContext.CurrentContext.TestDirectory + relativePath);
        }
        
        protected DxfDocument CreateCorrectPlainDxfDocument()
        {
            var polylines = new[]
                            {
                                GiveMe.DxfPolyline
                                      .WithVertex(0, 150)
                                      .WithVertex(150, 0)
                                      .WithVertex(0, -150)
                                      .WithVertex(-150, 0)
                                      .Please()
                            };
            
            var dxfDocument = GiveMe.DxfDocument
                                     .WithCircle(200)
                                     .WithPolylines(polylines)
                                     .Please();

            return dxfDocument;
        }
    }
}