using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Numerics;
using ATB.DxfToNcConverter.Services;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter
{
    internal static class EntryPoint
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
            ConfigureLogging(debug, whatIf);
            var logger = LogManager.GetCurrentClassLogger();
            
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
                logger.Info("DxfToNcConverter started...");
                systems.Init();
                systems.Run();
            }
            catch (Exception e)
            {
                logger.Fatal(e);
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                logger.Info("DxfToNcConverter closed.");
            }
        }

        private static void ConfigureLogging(bool isDebug, bool isWhatIf)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logInFileTarget = new NLog.Targets.FileTarget("logFileTarget") { FileName = "${baseDir}\\logs\\${shortdate}.log" };
            var logInConsoleTarget = new NLog.Targets.ConsoleTarget("logConsoleTarget");

            config.AddRule(isDebug || isWhatIf ? LogLevel.Trace : LogLevel.Info, LogLevel.Fatal, logInConsoleTarget);
            config.AddRule(isDebug || isWhatIf ? LogLevel.Trace : LogLevel.Info, LogLevel.Fatal, logInFileTarget);

            LogManager.Configuration = config;
        }
    }
}