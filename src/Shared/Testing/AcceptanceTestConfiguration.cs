// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;

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

        return new AcceptanceTestConfiguration(
            coreConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "Workflow__ConnectionString"),
                suffix: suffix),
            securityConnectionString: AddDatabaseSuffix(
                connectionString: ReadRequiredValue(
                    variableName: "Security__ConnectionString"),
                suffix: suffix),
            securityDecryptionKey: ReadRequiredValue(
                variableName: "Security__DecryptionKey"));
    }

    private static string ReadRequiredValue(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variable: variableName)
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
            new(connectionString: connectionString)
            {
                Encrypt = true,
                TrustServerCertificate = true
            };

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Acceptance test connection strings must name a database.");
        }

        builder.InitialCatalog = $"{builder.InitialCatalog}{suffix}";
        return builder.ConnectionString;
    }
}