// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Eventing;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Dependencies;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Models;
using Xunit;

namespace Web.AcceptanceTests.Infrastructure;

public sealed class IntegrationAcceptanceFixture : IAsyncLifetime
{
    private const string DecryptionKey = "000000000000000000000000000000000000000000000000";

    private readonly HttpClientHandler insecureHttpHandler = new()
    {
        AutomaticDecompression = DecompressionMethods.All,
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    private AcceptanceDatabaseManager databaseManager;
    private ServiceProvider databaseServices;
    private ExternalProcessApplication webApplication;
    private ExternalProcessApplication hostedServicesApplication;
    private ExternalProcessApplication workflowApplication;
    private readonly string repositoryRoot = FindRepositoryRoot();
    private readonly string buildConfiguration = ResolveBuildConfiguration();
    private string artifactsRoot;
    private string lastHealthProbeFailure;

    internal AcceptanceSettings Settings { get; private set; }

    public IServiceProvider DatabaseServices => databaseServices;

    public Uri WebBaseAddress { get; private set; }

    public Uri HostedServicesBaseAddress { get; private set; }

    public Uri WorkflowBaseAddress { get; private set; }

    public HttpClient WebClient { get; private set; }

    public HttpClient HostedServicesClient { get; private set; }

    public string WebOutput => webApplication?.Output ?? string.Empty;

    public string HostedServicesOutput => hostedServicesApplication?.Output ?? string.Empty;

    public string WorkflowOutput => workflowApplication?.Output ?? string.Empty;

    public async Task InitializeAsync()
    {
        Settings = new AcceptanceSettings
        {
            CoreConnectionString = AddDatabaseSuffix(variableName: "CCODER_ACCEPTANCE_CORE_CONNECTION_STRING", suffix: "integration"),
            SsoConnectionString = AddDatabaseSuffix(variableName: "CCODER_ACCEPTANCE_SSO_CONNECTION_STRING", suffix: "integration"),
            DecryptionKey = DecryptionKey,
        };

        int webHttpsPort = FindFreePort();
        int hostedServicesHttpPort = FindFreePort();
        int workflowHttpPort = FindFreePort();

        WebBaseAddress = new Uri($"https://localhost:{webHttpsPort}/");
        HostedServicesBaseAddress = new Uri($"http://localhost:{hostedServicesHttpPort}/");
        WorkflowBaseAddress = new Uri($"http://localhost:{workflowHttpPort}/api/");

        artifactsRoot = Path.Combine(path1: repositoryRoot, path2: "artifacts", path3: "workflow-integration", path4: Guid.NewGuid()
            .ToString(format: "N"));
        string workflowOutputDirectory = Path.Combine(path1: artifactsRoot, path2: "Workflow");
        string hostedServicesOutputDirectory = Path.Combine(path1: artifactsRoot, path2: "HostedServices");
        string webOutputDirectory = Path.Combine(path1: artifactsRoot, path2: "Web");

        Directory.CreateDirectory(path: workflowOutputDirectory);
        Directory.CreateDirectory(path: hostedServicesOutputDirectory);
        Directory.CreateDirectory(path: webOutputDirectory);

        databaseServices = CreateDatabaseServices(settings: Settings);
        databaseManager = new AcceptanceDatabaseManager(databaseServices);
        await databaseManager.ResetDatabasesAsync();
        await SeedBaselineUsersAsync();

        await BuildApplicationAsync(projectPath: "src\\Apps\\Workflow\\Workflow.csproj", outputDirectory: workflowOutputDirectory, intermediateDirectory: Path.Combine(path1: artifactsRoot, path2: "obj", path3: "Workflow"));
        await BuildApplicationAsync(projectPath: "src\\Workflow.HostedServices\\Workflow.HostedServices.csproj", outputDirectory: hostedServicesOutputDirectory, intermediateDirectory: Path.Combine(path1: artifactsRoot, path2: "obj", path3: "HostedServices"));
        await BuildApplicationAsync(projectPath: "src\\Workflow.Web\\Workflow.Web.csproj", outputDirectory: webOutputDirectory, intermediateDirectory: Path.Combine(path1: artifactsRoot, path2: "obj", path3: "Web"));

        workflowApplication = new ExternalProcessApplication("Workflow");
        await workflowApplication.StartAsync(
fileName: ResolveFuncExecutablePath(),
arguments: $"start --port {workflowHttpPort} --csharp --no-build",
workingDirectory: workflowOutputDirectory,
environmentVariables: new Dictionary<string, string>
{
    ["FUNCTIONS_WORKER_RUNTIME"] = "dotnet-isolated"
},
            readinessProbe: () => ProbeHealthAsync(baseAddress: WorkflowBaseAddress),
            timeout: TimeSpan.FromMinutes(minutes: 2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        Dictionary<string, string> hostedServicesEnvironment = CreateCommonEnvironment();
        hostedServicesEnvironment["ASPNETCORE_URLS"] = HostedServicesBaseAddress.ToString();
        hostedServicesEnvironment["Settings__sslPort"] = WebBaseAddress.Port.ToString();

        hostedServicesApplication = new ExternalProcessApplication("HostedServices");
        await hostedServicesApplication.StartAsync(
fileName: "dotnet",
arguments: $"\"{Path.Combine(path1: hostedServicesOutputDirectory, path2: "Workflow.HostedServices.dll")}\"",
workingDirectory: hostedServicesOutputDirectory,
environmentVariables: hostedServicesEnvironment,
            readinessProbe: () => ProbeHealthAsync(baseAddress: HostedServicesBaseAddress),
            timeout: TimeSpan.FromMinutes(minutes: 2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        Dictionary<string, string> webEnvironment = CreateCommonEnvironment();
        AddHttpsCertificateEnvironment(environment: webEnvironment);
        webEnvironment["ASPNETCORE_URLS"] = WebBaseAddress.ToString();
        webEnvironment["Settings__sslPort"] = webHttpsPort.ToString();
        webEnvironment["Services__HostedServices"] = HostedServicesBaseAddress.ToString();

        webApplication = new ExternalProcessApplication("Web");
        await webApplication.StartAsync(
fileName: "dotnet",
arguments: $"\"{Path.Combine(path1: webOutputDirectory, path2: "Workflow.Web.dll")}\"",
workingDirectory: webOutputDirectory,
environmentVariables: webEnvironment,
            readinessProbe: () => ProbeHealthAsync(baseAddress: WebBaseAddress, useInsecureHandler: true),
            timeout: TimeSpan.FromMinutes(minutes: 2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        WebClient = CreateClient(baseAddress: WebBaseAddress, useInsecureHandler: true);
        HostedServicesClient = CreateClient(baseAddress: HostedServicesBaseAddress, useInsecureHandler: false);
    }

    public async Task DisposeAsync()
    {
        WebClient?.Dispose();
        HostedServicesClient?.Dispose();

        if (webApplication is not null)
        {
            await webApplication.DisposeAsync();
        }

        if (hostedServicesApplication is not null)
        {
            await hostedServicesApplication.DisposeAsync();
        }

        if (workflowApplication is not null)
        {
            await workflowApplication.DisposeAsync();
        }

        if (databaseManager is not null)
        {
            await databaseManager.DropDatabasesAsync();
        }

        if (databaseServices is not null)
        {
            await databaseServices.DisposeAsync();
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(value: artifactsRoot) && Directory.Exists(path: artifactsRoot))
            {
                Directory.Delete(path: artifactsRoot, recursive: true);
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }

    private async Task SeedBaselineUsersAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();
        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>()
            .CreateCoreContext();
        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);

        await AcceptanceUserSeeder.EnsureCoreUserAsync(core: core, id: "Guest", displayName: "Guest", email: string.Empty);
        await AcceptanceUserSeeder.EnsureCoreUserAsync(core: core, id: "admin", displayName: "Acceptance Admin", email: "admin@localhost");
        await AcceptanceUserSeeder.EnsureSsoUserAsync(sso: sso, id: "Guest", displayName: "Guest", email: string.Empty);
        await AcceptanceUserSeeder.EnsureSsoUserAsync(sso: sso, id: "admin", displayName: "Acceptance Admin", email: "admin@localhost");
    }

    private static ServiceProvider CreateDatabaseServices(AcceptanceSettings settings)
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddEventing();
        services.AddSingleton(
implementationInstance: new Config
{
    ConnectionStrings = new Dictionary<string, string>
    {
        ["Core"] = settings.CoreConnectionString,
        ["SSO"] = settings.SsoConnectionString
    },
    Settings = new Dictionary<string, string>
    {
        ["DecryptionKey"] = settings.DecryptionKey,
        ["enableExternalEventing"] = "true"
    },
    Services = new Dictionary<string, string>()
});
        services.AddScoped<ISecurityDbContextFactory>(
            provider => new MSSQLSecurityDbContextFactory(settings.SsoConnectionString)
            {
                GetAuthInfo = ignoreAuthInfo => ignoreAuthInfo
                    ? new SSOAuthInfo { SSOUserId = "admin" }
                    : provider.GetService<ISSOAuthInfo>()
            });
        services.AddCoreData(connectionString: settings.CoreConnectionString);
        services.AddAppSecurityWeb();
        return services.BuildServiceProvider(validateScopes: false);
    }

    private Dictionary<string, string> CreateCommonEnvironment() =>
        new()
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Acceptance",
            ["ConnectionStrings__Core"] = Settings.CoreConnectionString,
            ["ConnectionStrings__SSO"] = Settings.SsoConnectionString,
            ["Settings__DecryptionKey"] = Settings.DecryptionKey,
            ["Settings__enableExternalEventing"] = "true",
            ["Services__Workflow"] = WorkflowBaseAddress.ToString(),
            ["Eventing__ProviderType"] = "Http",
            ["Eventing__Http__MaxConcurrency"] = "1"
        };

    private async Task BuildApplicationAsync(string projectPath, string outputDirectory, string intermediateDirectory)
    {
        string projectIntermediateDirectory = Path.Combine(path1: intermediateDirectory, path2: "$(MSBuildProjectName)");
        string outputProperties =
            $"-p:OutputPath=\"{FormatMsBuildPath(path: outputDirectory, trailingSlash: false)}\" " +
            $"-p:IntermediateOutputPath=\"{FormatMsBuildPath(path: projectIntermediateDirectory, trailingSlash: true)}\"";

        await RunCommandAsync(fileName: "dotnet", arguments: $"restore {projectPath} {outputProperties}");
        await RunCommandAsync(
fileName: "dotnet",
arguments: $"build {projectPath} --no-restore -c {buildConfiguration} -m:1 " +
            $"-p:BuildInParallel=false -p:UseSharedCompilation=false {outputProperties}");
    }

    private static string ResolveBuildConfiguration()
    {
        DirectoryInfo targetFrameworkDirectory = new(AppContext.BaseDirectory);
        return targetFrameworkDirectory.Parent?.Name ?? "Debug";
    }

    private async Task RunCommandAsync(string fileName, string arguments)
    {
        StringBuilder output = new();

        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = repositoryRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                output.AppendLine(value: args.Data);
            }
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                output.AppendLine(value: args.Data);
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException($"Failed to start command '{fileName} {arguments}'.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Command '{fileName} {arguments}' failed with exit code {process.ExitCode}.{Environment.NewLine}{output}");
        }
    }

    private async Task<bool> ProbeHealthAsync(Uri baseAddress, bool useInsecureHandler = false)
    {
        using HttpClient client = CreateClient(baseAddress: baseAddress, useInsecureHandler: useInsecureHandler);
        Uri healthUri = new(baseAddress, "Health");

        try
        {
            using HttpResponseMessage response = await client.GetAsync(requestUri: "Health");
            string content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && string.Equals(a: content, b: "OK", comparisonType: StringComparison.Ordinal))
            {
                lastHealthProbeFailure = null;
                return true;
            }

            lastHealthProbeFailure =
                $"GET {healthUri} returned {(int)response.StatusCode} {response.StatusCode} with body '{content}'.";
            return false;
        }
        catch (Exception exception)
        {
            lastHealthProbeFailure = $"GET {healthUri} failed: {exception.GetType().FullName}: {exception.Message}";
            return false;
        }
    }

    private HttpClient CreateClient(Uri baseAddress, bool useInsecureHandler)
    {
        HttpClient client = useInsecureHandler
            ? new HttpClient(insecureHttpHandler, disposeHandler: false)
            : new HttpClient();

        client.BaseAddress = baseAddress;
        client.Timeout = TimeSpan.FromMinutes(minutes: 2);
        return client;
    }

    private void AddHttpsCertificateEnvironment(Dictionary<string, string> environment)
    {
        string certificatePath = Path.Combine(path1: artifactsRoot, path2: "localhost-https.pfx");
        string certificatePassword = Guid.NewGuid()
            .ToString(format: "N");

        using RSA rsa = RSA.Create(keySizeInBits: 2048);
        CertificateRequest request = new(
            "CN=localhost",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        SubjectAlternativeNameBuilder subjectAlternativeNameBuilder = new();
        subjectAlternativeNameBuilder.AddDnsName(dnsName: "localhost");
        subjectAlternativeNameBuilder.AddIpAddress(ipAddress: IPAddress.Loopback);
        subjectAlternativeNameBuilder.AddIpAddress(ipAddress: IPAddress.IPv6Loopback);

        request.CertificateExtensions.Add(item: subjectAlternativeNameBuilder.Build());
        request.CertificateExtensions.Add(item: new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(
item: new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                critical: true));
        request.CertificateExtensions.Add(
item: new X509EnhancedKeyUsageExtension([new Oid("1.3.6.1.5.5.7.3.1")], critical: false));

        using X509Certificate2 certificate = request.CreateSelfSigned(
notBefore: DateTimeOffset.UtcNow.AddMinutes(minutes: -5),
notAfter: DateTimeOffset.UtcNow.AddDays(days: 1));

        File.WriteAllBytes(path: certificatePath, bytes: certificate.Export(contentType: X509ContentType.Pkcs12, password: certificatePassword));

        environment["ASPNETCORE_Kestrel__Certificates__Default__Path"] = certificatePath;
        environment["ASPNETCORE_Kestrel__Certificates__Default__Password"] = certificatePassword;
    }

    private static int FindFreePort()
    {
        using System.Net.Sockets.TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(path: Path.Combine(path1: directory.FullName, path2: "src", path3: "cCoder.Workflow.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the cCoder.Workflow repository root.");
    }

    private static string AddDatabaseSuffix(string variableName, string suffix)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(variable: variableName, target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variable: variableName, target: EnvironmentVariableTarget.Machine)
            ?? ReadConfiguredConnectionString(variableName: variableName);

        if (string.IsNullOrWhiteSpace(value: connectionString))
        {
            return string.Empty;
        }

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        if (!string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            builder.InitialCatalog = $"{builder.InitialCatalog}-workflow-{suffix}";
        }

        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        string connectionName = variableName.Contains(value: "CORE", comparisonType: StringComparison.OrdinalIgnoreCase)
            ? "Core"
            : "SSO";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: AppContext.BaseDirectory)
            .AddJsonFile(path: "appsettings.testing.json", optional: true)
            .Build();

        return configuration.GetConnectionString(name: connectionName) ?? string.Empty;
    }

    private static string FormatMsBuildPath(string path, bool trailingSlash)
    {
        string formattedPath = path.Replace(oldChar: '\\', newChar: '/');

        if (trailingSlash && !formattedPath.EndsWith(value: '/'))
        {
            formattedPath += '/';
        }

        return formattedPath;
    }

    private string GetHealthProbeDiagnostics() =>
        lastHealthProbeFailure ?? "No health probe failure was recorded.";

    private static string ResolveFuncExecutablePath()
    {
        string roamingNpmFunc = Path.Combine(
path1: Environment.GetFolderPath(folder: Environment.SpecialFolder.ApplicationData),
path2: "npm",
path3: "func.cmd");

        if (File.Exists(path: roamingNpmFunc))
        {
            return roamingNpmFunc;
        }

        return "func";
    }
}

[CollectionDefinition(Name)]
public sealed class IntegrationAcceptanceCollection : ICollectionFixture<IntegrationAcceptanceFixture>
{
    public const string Name = "Integration acceptance";
}