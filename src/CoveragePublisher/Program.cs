﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Azure.Pipelines.CoveragePublisher.Model;
using Microsoft.Azure.Pipelines.CoveragePublisher.Parsers;
using Microsoft.Azure.Pipelines.CoveragePublisher.Publishers.AzurePipelines;

namespace Microsoft.Azure.Pipelines.CoveragePublisher
{
    public class Program
    {
        private static CancellationTokenSource _cancellationTokenSource;
        private static bool publishSuccess = false;

        public static void Main(string[] args)
        {
            var argsProcessor = new ArgumentsProcessor();
            var config = argsProcessor.ProcessCommandLineArgs(args);

            _cancellationTokenSource = new CancellationTokenSource();

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            if (config != null)
            {
                ConfigureLogging(config);
                ProcessCoverage(config, _cancellationTokenSource.Token);
            }
        }

        private static void ConfigureLogging(PublisherConfiguration config)
        {
            if (config.TraceLogging)
            {
                var fileName = DateTime.UtcNow.ToString("CoveragePublisher.yyyy-MM-dd.HH-mm-ss." + Process.GetCurrentProcess().Id + ".log");
                var logFilePath = Path.Combine(Path.GetTempPath(), fileName);

                var listener = new TextWriterTraceListener(logFilePath);

                TraceLogger.Instance.AddListener(listener);
            }
        }

        private static void ProcessCoverage(PublisherConfiguration config, CancellationToken cancellationToken)
        {
            // Currently the publisher only works for azure pipelines, so we simply instansiate for Azure Pipelines
            var context = AzurePipelinesPublisher.ExecutionContext;
            AzurePipelinesPublisher publisher = null;

            try
            {
                publisher = new AzurePipelinesPublisher();
            }
            catch (Exception ex)
            {
                context.ConsoleLogger.Error(string.Format(Resources.CouldNotConnectToAzurePipelines, ex));
            }

            var processor = new CoverageProcessor(publisher, context);

            // By default wait for 2 minutes for coverage to publish
            var publishTimedout = processor.ParseAndPublishCoverage(config, cancellationToken, new Parser(config, context))
                                           .Wait(config.TimeoutInSeconds * 1000, cancellationToken);

            if(publishTimedout)
            {
                _cancellationTokenSource.Cancel();
            }
            else
            {
                publishSuccess = true;
            }
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            if (!publishSuccess)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
