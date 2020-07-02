using System.IO;
using ATB.DxfToNcConverter.Tests.Fakes;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests
{
    public abstract class TestBase
    {
        protected ConfigurationServiceStub ConfigurationServiceStub { get; private set; }
        protected DxfServiceStub DxfServiceStub { get; private set; }
        protected FileSystemServiceStub FileSystemServiceStub { get; private set; }
        
        [SetUp]
        protected virtual void Setup()
        {
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
            var dxfDocument = new DxfDocument();
            
            dxfDocument.AddEntity(new Circle(Vector2.Zero, 200));
            dxfDocument.AddEntity(new Circle(Vector2.Zero, 100));
            
            var vertexes = new []
                           {
                               new LwPolylineVertex(0, 150), 
                               new LwPolylineVertex(150, 0), 
                               new LwPolylineVertex(0, -150), 
                               new LwPolylineVertex(-150, 0)
                           };
            
            var polyline = new LwPolyline(vertexes, true);

            dxfDocument.AddEntity(polyline);

            return dxfDocument;
        }
    }
}