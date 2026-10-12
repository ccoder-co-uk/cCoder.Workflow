// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Brokers;
using cCoder.Workflow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Foundations;

public sealed partial class WorkflowAttemptRelationalTests
{
    [Fact]
    public async Task ShouldAllowOnlyOneCompareExchangeAndRejectStaleContextInSqlServerAsync()
    {
        // Given
        string databaseName = "WorkflowAttemptTests_" + Guid.NewGuid()
                .ToString(format: "N");

        string server = Environment.GetEnvironmentVariable(variable: "WORKFLOW_TEST_SQL_SERVER") ?? @"(localdb)\MSSQLLocalDB";

        Assert.True(condition: server == @"(localdb)\MSSQLLocalDB" || server.StartsWith(value: @"np:\\.\pipe\LOCALDB#", comparisonType: StringComparison.OrdinalIgnoreCase));

        string connectionString = "Server=" + server + ";Database=" + databaseName
            + ";Integrated Security=true;TrustServerCertificate=true;Connect Timeout=15";

        Mock<ICoreContextFactory> factory = new();

        factory.Setup(expression: value => value.CreateCoreContext())
            .Returns(valueFunction: () => new AttemptContext(connectionString: connectionString));

        await using AttemptContext setup = new(connectionString: connectionString);

        await setup.Database.EnsureCreatedAsync();

        try
        {
            FlowInstanceData initial = new()
            {
                Id = Guid.NewGuid(), FlowDefinitionId = Guid.NewGuid(), Caller = "caller",
                State = "AwaitingExecution", ContextString = "{\"ExecutionAttemptId\":\"" + Guid.NewGuid() + "\"}",
                Start = DateTimeOffset.UtcNow
            };

            setup.FlowInstances.Add(entity: initial);

            await setup.SaveChangesAsync();

            WorkflowInstanceManagementBroker broker = new(coreContextFactory: factory.Object);

            WorkflowInstanceExchange failure = new()
            {
                Expected = initial,
                Replacement = new FlowInstanceData
                {
                    State = "Failed", ContextString = initial.ContextString, Start = initial.Start, End = DateTimeOffset.UtcNow
                }
            };

            // When
            int[] results = await Task.WhenAll(tasks: [
                broker.CompareExchangeInstanceAsync(workflowInstanceExchange: failure)
                    .AsTask(),
                broker.CompareExchangeInstanceAsync(workflowInstanceExchange: failure)
                    .AsTask()]);

            // Then
            Assert.Equal(expected: 1, actual: results.Sum());

            await using AttemptContext read = new(connectionString: connectionString);

            FlowInstanceData failed = await read.FlowInstances.AsNoTracking()
                .SingleAsync();

            Assert.Equal(expected: "Failed", actual: failed.State);

            Assert.Equal(expected: "caller", actual: failed.Caller);

            // Given
            await read.FlowInstances.ExecuteUpdateAsync(setPropertyCalls: setters => setters
                .SetProperty(propertyExpression: value => value.State, valueExpression: "AwaitingExecution")
                .SetProperty(propertyExpression: value => value.ContextJson, valueExpression: System.Text.Encoding.UTF8.GetBytes(s: "{\"ExecutionAttemptId\":\"" + Guid.NewGuid() + "\"}")));

            // When
            int staleWrite = await broker.CompareExchangeInstanceAsync(workflowInstanceExchange: failure);

            // Then
            Assert.Equal(expected: 0, actual: staleWrite);

        }
        finally
        {
            await setup.Database.EnsureDeletedAsync();

        }
    }

    private sealed class AttemptContext(string connectionString)
        : CoreDataContext(Mock.Of<ICoreAuthInfo>(), new DataConfiguration { ConnectionString = connectionString }, NullLogger<CoreDataContext>.Instance)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (Type type in modelBuilder.Model.GetEntityTypes()
                .Select(selector: value => value.ClrType)
                .ToArray())
            {
                if (type != typeof(FlowInstanceData))
                {
                    modelBuilder.Ignore(type: type);

                }
            }

            modelBuilder.Entity<FlowInstanceData>()
                .Ignore(propertyExpression: value => value.FlowDefinition);

            modelBuilder.Entity<FlowInstanceData>()
                .Ignore(propertyExpression: value => value.ContextString);

            modelBuilder.Entity<FlowInstanceData>()
                .HasKey(keyExpression: value => value.Id);

        }
    }
}