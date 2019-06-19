﻿
using System;
using Microsoft.Azure.Pipelines.CoveragePublisher.Model;
using Microsoft.Azure.Pipelines.CoveragePublisher.Publishers.AzurePipelines;

namespace CoveragePublisher.Tests
{
    public class TestPipelinesExecutionContext : IPipelinesExecutionContext
    {
        private Guid _projectId;
        public TestPipelinesExecutionContext(ILogger consoleLogger)
        {
            ConsoleLogger = consoleLogger;
            _projectId = Guid.NewGuid();
        }

        public int BuildId => 1234;

        public long ContainerId => 12345;

        public string AccessToken => "accesstoken";

        public string CollectionUri => "collectionuri";

        public Guid ProjectId => _projectId;

        public ILogger ConsoleLogger { get; private set; }
    }
}