﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Numerics;
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
                                  new Option<bool>(new[] {"-d", "--debug"}, "Debug mode. Provides additional logging information while running."),
                                  new Option<bool>(new[] {"-wi", "--what-if"}, "What-if mode. It provides additional logging information while running but not performs real converting."),
                                  new Argument<string>("directory", Directory.GetCurrentDirectory, "Working directory."),
                                  new Option<double>(new [] {"-hdt", "--hole-drill-time"}, () => 1.5, "Drill down moving time in seconds."),
                                  new Option<double>(new [] {"-epx", "--end-position-x"}, () => 300, "X position at the end of program execution.")
                              };

            rootCommand.Description = "This application creates NC files from DXF files with corresponding file names." +
                                      "\nEach polyline vertex in DXF file will be a drill point in creating NC program." +
                                      "\n\nDXF files restrictions:" +
                                      "\n\t- Must contain at least one circle element;" +
                                      "\n\t- Must contain only closed polylines;" +
                                      "\n\t- All polylines must be inside the biggest circle.";

            rootCommand.Handler = CommandHandler.Create<bool, bool, string, double, double>(Run);
            return rootCommand.Invoke(args);
        }

        private static void Run(bool debug, bool whatIf, string directory, double holeDrillTime, double endPositionX) // Argument names are important for arguments mapping
        {
            ConfigureLogging(debug, whatIf);
            var logger = LogManager.GetCurrentClassLogger();
            
            logger.Info("DxfToNcConverter started...");

            if (debug)
            {
                logger.Info("Debug mode activated.");
            }
            
            if (whatIf)
            {
                logger.Info("What-If mode activated.");
            }
            
            var world = new EcsWorld();
            var systems = new EcsSystems(world);
            
            var configurationService = new ConfigurationService(debug, 
                                                                whatIf, 
                                                                directory, 
                                                                new netDxf.Vector2(endPositionX, 0), 
                                                                holeDrillTime);
            var fileSystemService = new FileSystemService();
            var dxfService = new DxfService();

            systems.Add(new DxfSearchProcessing())
                   .Add(new DxfLoadProcessing())
                   .Add(new DxfValidationProcessing())
                   .Add(new DxfParseProcessing())
                   .Add(new NcBuildProcessing())
                   .Add(new NcSaveProcessing(), nameof(NcSaveProcessing))
                   
                   .Inject(configurationService)
                   .Inject(fileSystemService)
                   .Inject(dxfService);

            if (whatIf)
            {
                var idx = systems.GetNamedRunSystem(nameof(NcSaveProcessing));
                systems.SetRunSystemState(idx, false);
            }

            var fatalError = false;

            try
            {
                systems.Init();
                systems.Run();
            }
            catch (Exception e)
            {
                fatalError = true;
                logger.Fatal(e);
                throw;
            }
            finally
            {
                systems.Destroy();
                world.Destroy();
                logger.Info("DxfToNcConverter closed.");

                if (fatalError || debug || whatIf)
                {
                    Console.WriteLine("Press any button to exit...");
                    Console.Read();
                }
            }
        }

        private static void ConfigureLogging(bool isDebug, bool isWhatIf)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var layout = new SimpleLayout("${longdate} [${level}] [${callsite}]: ${message}");

            var logInConsoleTarget = new NLog.Targets.ColoredConsoleTarget("logConsoleTarget")
                                     {
                                         Layout = layout
                                     };
            
            var logInFileTarget = new NLog.Targets.FileTarget("logFileTarget")
                                  {
                                      FileName = "${baseDir}\\logs\\${shortdate}.log", 
                                      Layout = layout
                                  };

            config.AddRule(isDebug || isWhatIf ? LogLevel.Trace : LogLevel.Info, LogLevel.Fatal, logInConsoleTarget);
            config.AddRule(isDebug ? LogLevel.Trace : LogLevel.Off, LogLevel.Fatal, logInFileTarget);

            LogManager.Configuration = config;
        }
    }
}