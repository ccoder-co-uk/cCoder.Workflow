// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Workflow;

internal static class Program
{
    private static async Task Main()
    {
        FunctionsApplicationBuilder builder =
            FunctionsApplication.CreateBuilder(args: []);

        builder.ConfigureFunctionsWebApplication();
        builder.Services.AddWorkflow(
            configuration: builder.Configuration);

        string acceptanceAuthorityPath = builder.Configuration["Workflow:AcceptanceAuthorityPath"];

        if (!string.IsNullOrWhiteSpace(value: acceptanceAuthorityPath)
            && builder.Environment.IsEnvironment(environmentName: "Acceptance"))
        {
            builder.Services.AddHttpClient(name: "WorkflowHttpClientBroker")
                .ConfigurePrimaryHttpMessageHandler(
                    () => CreateAcceptanceHandler(authorityPath: acceptanceAuthorityPath));
        }

        await builder
            .Build()
            .RunAsync();
    }

    private static SocketsHttpHandler CreateAcceptanceHandler(string authorityPath)
    {
        X509ChainPolicy policy = new()
        {
            TrustMode = X509ChainTrustMode.CustomRootTrust,
            RevocationMode = X509RevocationMode.NoCheck
        };

        policy.CustomTrustStore.Add(
            certificate: X509CertificateLoader.LoadCertificateFromFile(path: authorityPath));
        policy.ApplicationPolicy.Add(oid: new Oid("1.3.6.1.5.5.7.3.1"));

        return new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            SslOptions = new SslClientAuthenticationOptions { CertificateChainPolicy = policy }
        };
    }
}