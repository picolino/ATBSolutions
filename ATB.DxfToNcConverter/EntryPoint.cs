using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Numerics;
using ATB.DxfToNcConverter.Services;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter
{
    static class EntryPoint
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
                              {
                                  new Option<bool>(new[] {"-d", "--debug"}, "Debug mode. Provides additional logging information while running."),
                                  new Option<bool>(new[] {"-wi", "--what-if"}, "What-if mode. It provides additional logging information while running but not performs real converting.")
                              };
            rootCommand.Description = "This application creates NC files from DXF files with corresponding file names.";
            rootCommand.Handler = CommandHandler.Create<bool, bool>(Run);
            return rootCommand.Invoke(args);
        }

        private static void Run(bool debug, bool whatIf) // Argument names are important for arguments mapping
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            
            var configurationService = new ConfigurationService(debug, 
                                                                whatIf, 
                                                                Directory.GetCurrentDirectory(), 
                                                                new Vector2(260, 0), 
                                                                1.5); // TODO: Move all hardcode values to args
            var fileSystemService = new FileSystemService();
            var dxfService = new DxfService();

            systems.Add(new DxfSearchProcessing())
                   .Add(new DxfLoadProcessing())
                   .Add(new DxfValidationProcessing())
                   .Add(new DxfParseProcessing())
                   .Add(new NcBuildProcessing())
                   .Add(new NcSaveProcessing())
                   
                   .Inject(configurationService)
                   .Inject(fileSystemService)
                   .Inject(dxfService);
            
            try
            {
                systems.Init();
                systems.Run();
            }
            catch (Exception e)
            {
                // TODO: Log error
                throw;
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
            }
        }
    }
}