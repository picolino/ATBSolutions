using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Numerics;
using ATB.DxfToNcConverter.Resources;
using ATB.DxfToNcConverter.Services;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NLog;
using NLog.Layouts;

namespace ATB.DxfToNcConverter
{
    internal static class EntryPoint
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
                              {
                                  new Option<bool>(new[] {"-d", "--debug"}, Common.DebugModeDescription),
                                  new Option<bool>(new[] {"-wi", "--what-if"}, Common.WhatIfModeDescription),
                                  new Argument<string>("directory", Directory.GetCurrentDirectory, Common.WorkingDirectoryDescription),
                                  new Option<double>(new [] {"-hdt", "--hole-drill-time"}, () => 1.5, Common.HoleDrillTimeDescription),
                                  new Option<double>(new [] {"-fdt", "--fasteners-drill-time"}, () => 0.5, Common.FastenersDrillTimeDescription),
                                  new Option<double>(new [] {"-epx", "--end-position-x"}, () => 300, Common.EndXPositionDescription),
                                  new Option<double>(new [] {"-spxo", "--start-point-x-offset"}, () => -57, Common.XPositionOffsetDescription)
                              };

            rootCommand.Description = Common.ProgramDescription;

            rootCommand.Handler = CommandHandler.Create<bool, bool, string, double, double, double, double>(Run);
            return rootCommand.Invoke(args);
        }

        private static void Run(bool debug, bool whatIf, string directory, double holeDrillTime, double fastenersDrillTime, double endPositionX, double startPointXOffset) // Argument names are important for arguments mapping
        {
            ConfigureLogging(debug, whatIf);
            var logger = LogManager.GetCurrentClassLogger();
            
            logger.Info(Logging.ProgramStarted);

            if (debug)
            {
                logger.Info(Logging.DebugModeActivated);
            }
            
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            
            var configurationService = new ConfigurationService(debug, 
                                                                whatIf, 
                                                                directory, 
                                                                new netDxf.Vector2(endPositionX, 0), 
                                                                holeDrillTime,
                                                                fastenersDrillTime,
                                                                startPointXOffset);
            var fileSystemService = new FileSystemService();
            var dxfService = new DxfService();

            var consoleStayOpenForce = false;

            systems.Add(new DxfSearchProcessing(() => consoleStayOpenForce = true))
                   .Add(new DxfLoadProcessing())
                   .Add(new DxfValidationProcessing(e => consoleStayOpenForce = true))
                   .Add(new DxfParseProcessing())
                   .Add(new NcBuildProcessing())
                   .Add(new NcSaveProcessing(), nameof(NcSaveProcessing))
                   
                   .Inject(configurationService)
                   .Inject(fileSystemService)
                   .Inject(dxfService);

            if (whatIf)
            {
                logger.Info(Logging.WhatIfModeActivated);
                
                var idx = systems.GetNamedRunSystem(nameof(NcSaveProcessing));
                systems.SetRunSystemState(idx, false);
            }

            try
            {
                systems.Init();
                systems.Run();
            }
            catch (Exception e)
            {
                consoleStayOpenForce = true;
                logger.Fatal(e);
                throw;
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                
                logger.Info(Logging.ProgramFinished);

                if (consoleStayOpenForce || debug || whatIf)
                {
                    Console.WriteLine(Logging.PressAnyKeyToExit);
                    Console.Read();
                }
            }
        }

        private static void ConfigureLogging(bool isDebug, bool isWhatIf)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logInConsoleTarget = new NLog.Targets.ColoredConsoleTarget("logConsoleTarget")
                                     {
                                         Layout = new SimpleLayout("${message}")
                                     };
            
            var logInFileTarget = new NLog.Targets.FileTarget("logFileTarget")
                                  {
                                      FileName = "${baseDir}\\logs\\${shortdate}.log", 
                                      Layout = new SimpleLayout("${longdate} [${level}] [${callsite}]: ${message}")
                                  };

            config.AddRule(isDebug || isWhatIf ? LogLevel.Trace : LogLevel.Info, LogLevel.Fatal, logInConsoleTarget);
            config.AddRule(isDebug ? LogLevel.Trace : LogLevel.Off, LogLevel.Fatal, logInFileTarget);

            LogManager.Configuration = config;
        }
    }
}