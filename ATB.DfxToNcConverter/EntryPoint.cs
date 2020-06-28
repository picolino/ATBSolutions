using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using ATB.DfxToNcConverter.Components;
using Leopotam.Ecs;

namespace ATB.DfxToNcConverter
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
            rootCommand.Description = "This application creates NC files from DFX files with corresponding file names.";
            rootCommand.Handler = CommandHandler.Create<bool, bool>(Run);
            return rootCommand.Invoke(args);
        }

        private static void Run(bool debug, bool whatIf) // Argument names are important for arguments mapping
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);

            var configurationEntity = world.NewEntity();
            ref var configurationComponent = ref configurationEntity.Get<Configuration>();
            configurationComponent.debug = debug;
            configurationComponent.whatIf = whatIf;
            
            // TODO: Add systems.
            
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