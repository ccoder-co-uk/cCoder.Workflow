// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace cCoder.Workflow.Testing;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration(
        string coreConnectionString,
        string securityConnectionString,
        string securityDecryptionKey)
    {
        CoreConnectionString = coreConnectionString;
        SecurityConnectionString = securityConnectionString;
        SecurityDecryptionKey = securityDecryptionKey;
    }

    internal string CoreConnectionString { get; }
    internal string SecurityConnectionString { get; }
    internal string SecurityDecryptionKey { get; }

    internal static AcceptanceTestConfiguration Load()
    {
        string suffix = $"-acceptance-{Guid.NewGuid():N}";

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(
                path: "appsettings.testing.json",
                optional: true)
            .Build();

        return new AcceptanceTestConfiguration(
            coreConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    configuration: configuration,
                    configurationKey: "CoreData:ConnectionString",
                    variableName: "CoreData__ConnectionString"),
                suffix: suffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    configuration: configuration,
                    configurationKey: "SecurityData:ConnectionString",
                    variableName: "SecurityData__ConnectionString"),
                suffix: suffix),
            securityDecryptionKey: ReadRequiredValue(
                configuration: configuration,
                configurationKey: "Security:DecryptionKey",
                variableName: "Security__DecryptionKey"));
    }

    private static string ReadRequiredValue(
        IConfiguration configuration,
        string configurationKey,
        string variableName)
    {
        string value =
            configuration[configurationKey]
            ?? Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }

    private static string AddDatabaseSuffix(
        string connectionString,
        string suffix)
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: connectionString);

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{builder.InitialCatalog}{suffix}";
        return builder.ConnectionString;
    }
}