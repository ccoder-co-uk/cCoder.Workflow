// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using System.Collections.Immutable;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using cCoder.Workflow.Engine.Models.Exceptions;
using cCoder.Workflow.Engine.Services.Foundations;
using FluentAssertions;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace cCoder.Workflow.Engine.Tests;

public sealed partial class ScriptServiceTests
{
    private readonly Mock<IRoslynScriptBroker> roslynScriptBrokerMock =
        new(behavior: MockBehavior.Strict);

    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();
    private readonly Assembly testAssembly = typeof(ScriptServiceTests).Assembly;

    [Fact]
    public async Task BuildScript_WhenEvaluationSucceeds_ReturnsValue()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: "return 7;",
                options: It.IsAny<ScriptOptions>(),
                globals: null,
                globalsType: null))
            .ReturnsAsync(value: 7);

        ScriptService service = CreateService();

        // When
        int result = await service.BuildScript<int>(
            code: "return 7;",
            imports: ["System"],
            log: (_, _) => { });

        // Then
        result
            .Should()
            .Be(expected: 7);
    }

    [Fact]
    public async Task BuildScript_WhenEvaluationFails_LogsAndReturnsDefault()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: null,
                globalsType: null))
            .ThrowsAsync(exception: new Exception(message: "compile failed"));

        List<(WorkflowLogLevel Level, string Message)> logs = [];
        ScriptService service = CreateService();

        // When
        int result = await service.BuildScript<int>(
            code: "invalid",
            imports: ["System"],
            log: (level, message) => logs.Add(item: (level, message)));

        // Then
        result
            .Should()
            .Be(expected: default);

        logs
            .Should()
            .HaveCount(expected: 2);

        logs
            .Select(selector: entry => entry.Message)
            .Should()
            .Contain(expected: "compile failed");
    }

    [Fact]
    public async Task BuildScript_WhenCompilationFails_LogsSourceAndReturnsDefault()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);

        CompilationErrorException failure = new(
            message: "compile failed",
            diagnostics: ImmutableArray<Diagnostic>.Empty);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: null,
                globalsType: null))
            .ThrowsAsync(exception: failure);

        List<(WorkflowLogLevel Level, string Message)> logs = [];
        ScriptService service = CreateService();

        // When
        int result = await service.BuildScript<int>(
            code: "invalid",
            imports: ["System"],
            log: (level, message) => logs.Add(item: (level, message)));

        // Then
        result
            .Should()
            .Be(expected: default);

        logs
            .Should()
            .HaveCount(expected: 3);
    }

    [Fact]
    public async Task Run_WhenEvaluationSucceeds_ReturnsValueAndLogsDetails()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        var globals = new { Value = 3 };

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: "return Value;",
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ReturnsAsync(value: 3);

        List<(WorkflowLogLevel Level, string Message)> logs = [];
        ScriptService service = CreateService();

        // When
        int result = await service.Run<int>(
            code: "return Value;",
            imports: ["System"],
            args: globals,
            log: (level, message) => logs.Add(item: (level, message)));

        // Then
        result
            .Should()
            .Be(expected: 3);

        logs
            .Should()
            .ContainSingle(
            predicate: entry => entry.Level == WorkflowLogLevel.Debug);
    }

    [Fact]
    public async Task Run_WhenNoResultIsRequested_ExecutesBooleanScript()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        object globals = new();

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<bool>(
                code: "DoWork();return true;",
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ReturnsAsync(value: true);

        ScriptService service = CreateService();

        // When
        await service.Run(
            code: "DoWork()",
            imports: ["System"],
            args: globals,
            log: null);

        // Then
        roslynScriptBrokerMock.Verify(expression: broker =>
            broker.EvaluateAsync<bool>(
                code: "DoWork();return true;",
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()),
            times: Times.Once);
    }

    [Fact]
    public async Task Run_WhenEvaluationThrowsNullReference_LogsAndMapsFailure()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        object globals = new();
        NullReferenceException failure = new(message: "missing value");
        failure.Data.Add(key: "item", value: "value");

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ThrowsAsync(exception: failure);

        List<string> logs = [];
        ScriptService service = CreateService();

        // When
        Func<Task> action = async () => await service.Run<int>(
            code: "return Missing;",
            imports: ["System"],
            args: globals,
            log: (_, message) => logs.Add(item: message));

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();

        logs
            .Should()
            .ContainSingle(
            predicate: message => message.Contains(
                value: "item: value",
                comparisonType: StringComparison.Ordinal));
    }

    [Fact]
    public async Task Run_WhenCompilationFails_LogsAndMapsFailure()
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        object globals = new();

        CompilationErrorException failure = new(
            message: "compile failed",
            diagnostics: ImmutableArray<Diagnostic>.Empty);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ThrowsAsync(exception: failure);

        List<string> logs = [];
        ScriptService service = CreateService();

        // When
        Func<Task> action = async () => await service.Run<int>(
            code: "invalid",
            imports: ["System"],
            args: globals,
            log: (_, message) => logs.Add(item: message));

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineServiceException>();

        logs
            .Should()
            .HaveCount(expected: 2);
    }

    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task Run_WhenDependencyFails_MapsGenericResultException(
        Exception exception,
        Type expectedType)
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        object globals = new();

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ThrowsAsync(exception: exception);

        ScriptService service = CreateService();

        // When
        Func<Task> action = async () => await service.Run<int>(
            code: "return Value;",
            imports: ["System"],
            args: globals,
            log: null);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(
        nameof(WorkflowRequestOrchestrationServiceTests.ExceptionMappings),
        MemberType = typeof(WorkflowRequestOrchestrationServiceTests))]
    public async Task Run_WhenDependencyFails_MapsNoResultException(
        Exception exception,
        Type expectedType)
    {
        // Given
        SetupReferences();
        SetupOptions(imports: ["System"]);
        object globals = new();

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<bool>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ThrowsAsync(exception: exception);

        ScriptService service = CreateService();

        // When
        Func<Task> action = async () => await service.Run(
            code: "DoWork()",
            imports: ["System"],
            args: globals,
            log: null);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public void Constructor_WhenAssemblyLoadFails_LogsWarningAndContinues()
    {
        // Given
        SetupReferences(files: ["missing.dll"]);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.LoadFile(
                path: "missing.dll"))
            .Throws(exception: new InvalidOperationException(
                message: "not loadable"));

        // When
        Action action = () => _ = CreateService();

        // Then
        action
            .Should()
            .NotThrow();

        loggingBrokerMock.Verify(expression: broker => broker.LogWarning(
            message: It.Is<string>(match: value => value.Contains(
                value: "Unable to load assembly",
                comparisonType: StringComparison.Ordinal)),
            args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public void Constructor_WhenAssemblyLoads_LogsDebugAndContinues()
    {
        // Given
        SetupReferences(files: ["additional.dll"]);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.LoadFile(
                path: "additional.dll"))
            .Returns(value: testAssembly);

        // When
        Action action = () => _ = CreateService();

        // Then
        action
            .Should()
            .NotThrow();

        loggingBrokerMock.Verify(expression: broker => broker.LogDebug(
            message: It.IsAny<string>(),
            args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public void Constructor_WhenReferenceDiscoveryFails_UsesCurrentAssemblies()
    {
        // Given
        roslynScriptBrokerMock
            .SetupSequence(expression: broker =>
                broker.GetCurrentAssemblies())
            .Throws(exception: new InvalidOperationException(
                message: "discovery failed"))
            .Returns(value: [testAssembly]);

        // When
        Action action = () => _ = CreateService();

        // Then
        action
            .Should()
            .NotThrow();

        loggingBrokerMock.Verify(expression: broker => broker.LogWarning(
            message: It.IsAny<string>(),
            args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public async Task Run_WhenReferenceInspectionFails_ContinuesWithoutReference()
    {
        // Given
        SetupReferences();

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.GetExportedTypes(
                assembly: testAssembly))
            .Throws(exception: new InvalidOperationException(
                message: "inspection failed"));

        SetupOptions(imports: ["System"]);
        object globals = new();

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.EvaluateAsync<int>(
                code: It.IsAny<string>(),
                options: It.IsAny<ScriptOptions>(),
                globals: globals,
                globalsType: globals.GetType()))
            .ReturnsAsync(value: 1);

        ScriptService service = CreateService();

        // When
        int result = await service.Run<int>(
            code: "return 1;",
            imports: ["System"],
            args: globals,
            log: null);

        // Then
        result
            .Should()
            .Be(expected: 1);
    }

    [Fact]
    public async Task BuildScript_WhenCodeIsMissing_ThrowsValidationException()
    {
        // Given
        SetupReferences();
        ScriptService service = CreateService();

        // When
        Func<Task> action = async () => await service.BuildScript<int>(
            code: null,
            imports: ["System"],
            log: (_, _) => { });

        // Then
        await action
            .Should()
            .ThrowAsync<WorkflowEngineValidationException>();
    }

    private ScriptService CreateService() =>
        new(
            roslynScriptBroker: roslynScriptBrokerMock.Object,
            loggingBroker: loggingBrokerMock.Object);

    private void SetupReferences(string[] files = null)
    {
        roslynScriptBrokerMock
            .Setup(expression: broker => broker.GetCurrentAssemblies())
            .Returns(value: [testAssembly]);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.GetExecutingAssembly())
            .Returns(value: testAssembly);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.GetFiles(
                path: It.IsAny<string>(),
                searchPattern: "*.dll"))
            .Returns(value: files ?? []);

        roslynScriptBrokerMock
            .Setup(expression: broker => broker.GetExportedTypes(
                assembly: testAssembly))
            .Returns(value: [typeof(string)]);
    }

    private void SetupOptions(string[] imports) =>
        roslynScriptBrokerMock
            .Setup(expression: broker => broker.BuildOptions(
                references: It.IsAny<IEnumerable<Assembly>>(),
                imports: imports))
            .Returns(value: ScriptOptions.Default);
}