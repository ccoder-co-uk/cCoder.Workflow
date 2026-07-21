using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using cCoder.AppSecurity;
using cCoder.Data;
using cCoder.Eventing;
using cCoder.Security.Data.EF;
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
            CoreConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_CORE_CONNECTION_STRING", "integration"),
            SsoConnectionString = AddDatabaseSuffix("CCODER_ACCEPTANCE_SSO_CONNECTION_STRING", "integration"),
            DecryptionKey = DecryptionKey,
        };

        int webHttpsPort = FindFreePort();
        int hostedServicesHttpPort = FindFreePort();
        int workflowHttpPort = FindFreePort();

        WebBaseAddress = new Uri($"https://localhost:{webHttpsPort}/");
        HostedServicesBaseAddress = new Uri($"http://localhost:{hostedServicesHttpPort}/");
        WorkflowBaseAddress = new Uri($"http://localhost:{workflowHttpPort}/api/");

        artifactsRoot = Path.Combine(repositoryRoot, "artifacts", "workflow-integration", Guid.NewGuid().ToString("N"));
        string workflowOutputDirectory = Path.Combine(artifactsRoot, "Workflow");
        string hostedServicesOutputDirectory = Path.Combine(artifactsRoot, "HostedServices");
        string webOutputDirectory = Path.Combine(artifactsRoot, "Web");

        Directory.CreateDirectory(workflowOutputDirectory);
        Directory.CreateDirectory(hostedServicesOutputDirectory);
        Directory.CreateDirectory(webOutputDirectory);

        databaseServices = CreateDatabaseServices(Settings);
        databaseManager = new AcceptanceDatabaseManager(databaseServices);
        await databaseManager.ResetDatabasesAsync();
        await SeedBaselineUsersAsync();

        await BuildApplicationAsync("src\\Apps\\Workflow\\Workflow.csproj", workflowOutputDirectory, Path.Combine(artifactsRoot, "obj", "Workflow"));
        await BuildApplicationAsync("src\\Workflow.HostedServices\\Workflow.HostedServices.csproj", hostedServicesOutputDirectory, Path.Combine(artifactsRoot, "obj", "HostedServices"));
        await BuildApplicationAsync("src\\Workflow.Web\\Workflow.Web.csproj", webOutputDirectory, Path.Combine(artifactsRoot, "obj", "Web"));

        workflowApplication = new ExternalProcessApplication("Workflow");
        await workflowApplication.StartAsync(
            ResolveFuncExecutablePath(),
            $"start --port {workflowHttpPort} --csharp --no-build",
            workflowOutputDirectory,
            new Dictionary<string, string>
            {
                ["FUNCTIONS_WORKER_RUNTIME"] = "dotnet-isolated"
            },
            readinessProbe: () => ProbeHealthAsync(WorkflowBaseAddress),
            timeout: TimeSpan.FromMinutes(2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        Dictionary<string, string> hostedServicesEnvironment = CreateCommonEnvironment();
        hostedServicesEnvironment["ASPNETCORE_URLS"] = HostedServicesBaseAddress.ToString();
        hostedServicesEnvironment["Settings__sslPort"] = WebBaseAddress.Port.ToString();

        hostedServicesApplication = new ExternalProcessApplication("HostedServices");
        await hostedServicesApplication.StartAsync(
            "dotnet",
            $"\"{Path.Combine(hostedServicesOutputDirectory, "Workflow.HostedServices.dll")}\"",
            hostedServicesOutputDirectory,
            hostedServicesEnvironment,
            readinessProbe: () => ProbeHealthAsync(HostedServicesBaseAddress),
            timeout: TimeSpan.FromMinutes(2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        Dictionary<string, string> webEnvironment = CreateCommonEnvironment();
        AddHttpsCertificateEnvironment(webEnvironment);
        webEnvironment["ASPNETCORE_URLS"] = WebBaseAddress.ToString();
        webEnvironment["Settings__sslPort"] = webHttpsPort.ToString();
        webEnvironment["Services__HostedServices"] = HostedServicesBaseAddress.ToString();

        webApplication = new ExternalProcessApplication("Web");
        await webApplication.StartAsync(
            "dotnet",
            $"\"{Path.Combine(webOutputDirectory, "Workflow.Web.dll")}\"",
            webOutputDirectory,
            webEnvironment,
            readinessProbe: () => ProbeHealthAsync(WebBaseAddress, useInsecureHandler: true),
            timeout: TimeSpan.FromMinutes(2),
            readinessDiagnostics: GetHealthProbeDiagnostics);

        WebClient = CreateClient(WebBaseAddress, useInsecureHandler: true);
        HostedServicesClient = CreateClient(HostedServicesBaseAddress, useInsecureHandler: false);
    }

    public async Task DisposeAsync()
    {
        WebClient?.Dispose();
        HostedServicesClient?.Dispose();

        if (webApplication is not null)
            await webApplication.DisposeAsync();

        if (hostedServicesApplication is not null)
            await hostedServicesApplication.DisposeAsync();

        if (workflowApplication is not null)
            await workflowApplication.DisposeAsync();

        if (databaseManager is not null)
            await databaseManager.DropDatabasesAsync();

        if (databaseServices is not null)
            await databaseServices.DisposeAsync();

        try
        {
            if (!string.IsNullOrWhiteSpace(artifactsRoot) && Directory.Exists(artifactsRoot))
                Directory.Delete(artifactsRoot, recursive: true);
        }
        catch
        {
            // best-effort cleanup
        }
    }

    private async Task SeedBaselineUsersAsync()
    {
        using IServiceScope scope = databaseServices.CreateScope();
        using var core = scope.ServiceProvider.GetRequiredService<ICoreContextFactory>().CreateCoreContext();
        using var sso = scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>().CreateDbContext(true);

        await AcceptanceUserSeeder.EnsureCoreUserAsync(core, "Guest", "Guest", string.Empty);
        await AcceptanceUserSeeder.EnsureCoreUserAsync(core, "admin", "Acceptance Admin", "admin@localhost");
        await AcceptanceUserSeeder.EnsureSsoUserAsync(sso, "Guest", "Guest", string.Empty);
        await AcceptanceUserSeeder.EnsureSsoUserAsync(sso, "admin", "Acceptance Admin", "admin@localhost");
    }

    private static ServiceProvider CreateDatabaseServices(AcceptanceSettings settings)
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddEventing();
        services.AddSingleton(
            new Config
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
        services.AddCoreData(settings.CoreConnectionString);
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
        string projectIntermediateDirectory = Path.Combine(intermediateDirectory, "$(MSBuildProjectName)");
        string outputProperties =
            $"-p:OutputPath=\"{FormatMsBuildPath(outputDirectory, trailingSlash: false)}\" " +
            $"-p:IntermediateOutputPath=\"{FormatMsBuildPath(projectIntermediateDirectory, trailingSlash: true)}\"";

        await RunCommandAsync("dotnet", $"restore {projectPath} {outputProperties}");
        await RunCommandAsync(
            "dotnet",
            $"build {projectPath} --no-restore -c {buildConfiguration} -m:1 " +
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
                output.AppendLine(args.Data);
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
                output.AppendLine(args.Data);
        };

        if (!process.Start())
            throw new InvalidOperationException($"Failed to start command '{fileName} {arguments}'.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"Command '{fileName} {arguments}' failed with exit code {process.ExitCode}.{Environment.NewLine}{output}");
    }

    private async Task<bool> ProbeHealthAsync(Uri baseAddress, bool useInsecureHandler = false)
    {
        using HttpClient client = CreateClient(baseAddress, useInsecureHandler);
        Uri healthUri = new(baseAddress, "Health");

        try
        {
            using HttpResponseMessage response = await client.GetAsync("Health");
            string content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode && string.Equals(content, "OK", StringComparison.Ordinal))
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
        client.Timeout = TimeSpan.FromMinutes(2);
        return client;
    }

    private void AddHttpsCertificateEnvironment(Dictionary<string, string> environment)
    {
        string certificatePath = Path.Combine(artifactsRoot, "localhost-https.pfx");
        string certificatePassword = Guid.NewGuid().ToString("N");

        using RSA rsa = RSA.Create(2048);
        CertificateRequest request = new(
            "CN=localhost",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        SubjectAlternativeNameBuilder subjectAlternativeNameBuilder = new();
        subjectAlternativeNameBuilder.AddDnsName("localhost");
        subjectAlternativeNameBuilder.AddIpAddress(IPAddress.Loopback);
        subjectAlternativeNameBuilder.AddIpAddress(IPAddress.IPv6Loopback);

        request.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build());
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                critical: true));
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension([new Oid("1.3.6.1.5.5.7.3.1")], critical: false));

        using X509Certificate2 certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));

        File.WriteAllBytes(certificatePath, certificate.Export(X509ContentType.Pkcs12, certificatePassword));

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
            if (File.Exists(Path.Combine(directory.FullName, "src", "cCoder.Workflow.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the cCoder.Workflow repository root.");
    }

    private static string AddDatabaseSuffix(string variableName, string suffix)
    {
        string connectionString =
            Environment.GetEnvironmentVariable(variableName)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine)
            ?? ReadConfiguredConnectionString(variableName);

        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        if (!string.IsNullOrWhiteSpace(builder.InitialCatalog))
            builder.InitialCatalog = $"{builder.InitialCatalog}-workflow-{suffix}";

        return builder.ConnectionString;
    }

    private static string ReadConfiguredConnectionString(string variableName)
    {
        string connectionName = variableName.Contains("CORE", StringComparison.OrdinalIgnoreCase)
            ? "Core"
            : "SSO";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.testing.json", optional: true)
            .Build();

        return configuration.GetConnectionString(connectionName) ?? string.Empty;
    }

    private static string FormatMsBuildPath(string path, bool trailingSlash)
    {
        string formattedPath = path.Replace('\\', '/');

        if (trailingSlash && !formattedPath.EndsWith('/'))
            formattedPath += '/';

        return formattedPath;
    }

    private string GetHealthProbeDiagnostics() =>
        lastHealthProbeFailure ?? "No health probe failure was recorded.";

    private static string ResolveFuncExecutablePath()
    {
        string roamingNpmFunc = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "npm",
            "func.cmd");

        if (File.Exists(roamingNpmFunc))
            return roamingNpmFunc;

        return "func";
    }
}

[CollectionDefinition(Name)]
public sealed class IntegrationAcceptanceCollection : ICollectionFixture<IntegrationAcceptanceFixture>
{
    public const string Name = "Integration acceptance";
}
