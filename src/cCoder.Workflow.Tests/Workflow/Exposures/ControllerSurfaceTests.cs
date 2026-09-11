// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

#pragma warning disable STXFORMAT001, STXFORMAT005, STXFORMAT009, STXFORMAT013, STXTEST005

using System.Reflection;
using cCoder.Workflow.Exposures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace cCoder.Workflow.Tests.Workflow.Exposures;

public sealed partial class ControllerSurfaceTests
{
    [Fact]
    public void Controllers_WhenInspected_DoNotExposePatchOrMergeActions()
    {
        // Given / When
        MethodInfo[] forbiddenActions = GetControllerActions()
            .Where(method => method.GetCustomAttributes<AcceptVerbsAttribute>()
                .SelectMany(attribute => attribute.HttpMethods)
                .Any(httpMethod =>
                    string.Equals(httpMethod, "PATCH", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(httpMethod, "MERGE", StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        // Then
        forbiddenActions.Should().BeEmpty();
    }

    [Fact]
    public void Controllers_WhenInspected_DoNotExposePerControllerMetadataActions()
    {
        // Given / When
        MethodInfo[] forbiddenActions = GetControllerActions()
            .Where(method => method.Name == "GetMetadata")
            .ToArray();

        // Then
        forbiddenActions.Should().BeEmpty();
    }

    private static IEnumerable<MethodInfo> GetControllerActions() =>
        typeof(CalendarController).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true } &&
                type.Name.EndsWith("Controller", StringComparison.Ordinal))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
}