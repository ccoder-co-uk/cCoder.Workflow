// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Brokers.Loggings;
using Microsoft.CodeAnalysis.Scripting;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class ScriptService(
    IRoslynScriptBroker roslynScriptBroker,
    ILoggingBroker loggingBroker)
    : IScriptService
{
    private readonly Assembly[] references = LoadReferences(
        roslynScriptBroker: roslynScriptBroker,
        loggingBroker: loggingBroker);

    public Task<T> BuildScript<T>(
        string code,
        string[] imports,
        Action<WorkflowLogLevel, string> log) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [code, imports, log]);

            return await ExecuteBuildScriptAsync<T>(
                code: code,
                imports: imports,
                log: log);
        });

    private async Task<T> ExecuteBuildScriptAsync<T>(
        string code,
        string[] imports,
        Action<WorkflowLogLevel, string> log)
    {
        try
        {
            ScriptOptions options = BuildScriptOptions(imports: imports);

            return await roslynScriptBroker.EvaluateAsync<T>(
                code: code,
                options: options);
        }
        catch (Exception exception)
        {
            log(
                arg1: WorkflowLogLevel.Error,
                arg2: "Script failed to compile.");

            log(
                arg1: WorkflowLogLevel.Error,
                arg2: exception.Message);

            if (exception is CompilationErrorException compilationError)
            {
                log(
                    arg1: WorkflowLogLevel.Error,
                    arg2: $"Source of the problem:{Environment.NewLine}{compilationError.Source}");
            }

            return default;
        }
    }

    public Task<T> Run<T>(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [code, imports, args, log]);

            return await ExecuteRunScriptAsync<T>(
                code: code,
                imports: imports,
                args: args,
                log: log);
        });

    private async Task<T> ExecuteRunScriptAsync<T>(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log)
    {
        try
        {
            IEnumerable<Assembly> requiredReferences =
                ResolveReferences(imports: imports);

            ScriptOptions options = roslynScriptBroker.BuildOptions(
                references: requiredReferences,
                imports: imports);

            if (log is not null)
            {
                string details =
                    $"{Environment.NewLine}Imports{Environment.NewLine}  "
                    + string.Join(
                        separator: $"{Environment.NewLine}  ",
                        value: imports)
                    + $"{Environment.NewLine}{Environment.NewLine}"
                    + $"References Needed{Environment.NewLine}  "
                    + string.Join(
                        separator: $"{Environment.NewLine}  ",
                        values: requiredReferences.Select(
                            selector: reference => reference.FullName));

                log(
                    arg1: WorkflowLogLevel.Debug,
                    arg2: details);
            }

            return await roslynScriptBroker.EvaluateAsync<T>(
                code: code,
                options: options,
                globals: args,
                globalsType: args?.GetType());
        }
        catch (NullReferenceException exception)
        {
            string target = exception.TargetSite is null
                ? "unknown"
                : $"(({exception.TargetSite.DeclaringType?.Name ?? "object"})object).{exception.TargetSite.Name}";

            List<string> context = [];

            foreach (object key in exception.Data.Keys)
            {
                context.Add(item: $"{key}: {exception.Data[key]}");
            }

            log?.Invoke(
                arg1: WorkflowLogLevel.Error,
                arg2: $"{exception.Message}{Environment.NewLine}"
                    + $"Context: {exception.Source}{Environment.NewLine}"
                    + $"Target: {target}{Environment.NewLine}"
                    + string.Join(
                        separator: Environment.NewLine,
                        values: context));

            throw;
        }
        catch (CompilationErrorException exception)
        {
            log?.Invoke(
                arg1: WorkflowLogLevel.Error,
                arg2: $"Compilation failed:{Environment.NewLine}"
                    + $"{exception.Message}{Environment.NewLine}"
                    + string.Join(
                        separator: Environment.NewLine,
                        values: exception.Diagnostics));

            throw;
        }
    }

    public Task Run(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [code, imports, args, log]);

            _ = await ExecuteRunScriptAsync<bool>(
                code: $"{code};return true;",
                imports: imports,
                args: args,
                log: log);
        });

    private static Assembly[] LoadReferences(
        IRoslynScriptBroker roslynScriptBroker,
        ILoggingBroker loggingBroker)
    {
        try
        {
            List<Assembly> loadedAssemblies = roslynScriptBroker
                .GetCurrentAssemblies()
                .Where(predicate: assembly => !assembly.IsDynamic)
                .ToList();

            Assembly currentAssembly =
                roslynScriptBroker.GetExecutingAssembly();

            string binDirectory = currentAssembly.Location.Replace(
                oldValue: currentAssembly.ManifestModule.Name,
                newValue: string.Empty,
                comparisonType: StringComparison.Ordinal);

            string[] assembliesToLoad = roslynScriptBroker.GetFiles(
                path: binDirectory,
                searchPattern: "*.dll")
                .Where(predicate: path => loadedAssemblies.All(
                    predicate: assembly => !string.Equals(
                        a: assembly.Location,
                        b: path,
                        comparisonType: StringComparison.OrdinalIgnoreCase)))
                .Where(predicate: path => !path.Contains(
                    value: "api-ms-win",
                    comparisonType: StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string assemblyPath in assembliesToLoad)
            {
                SafelyLoadAssembly(
                    roslynScriptBroker: roslynScriptBroker,
                    loggingBroker: loggingBroker,
                    loadedAssemblies: loadedAssemblies,
                    assemblyPath: assemblyPath);
            }

            return loadedAssemblies.ToArray();
        }
        catch (Exception exception)
        {
            loggingBroker.LogWarning(
                message: "Script runner may be missing references but will continue: {Message}",
                args: [exception.Message]);

            return roslynScriptBroker
                .GetCurrentAssemblies()
                .Where(predicate: assembly => !assembly.IsDynamic)
                .ToArray();
        }
    }

    private ScriptOptions BuildScriptOptions(
        string[] imports) =>
        roslynScriptBroker.BuildOptions(
            references: ResolveReferences(imports: imports),
            imports: imports);

    private IEnumerable<Assembly> ResolveReferences(
        string[] imports) =>
        references.Where(predicate: reference =>
        {
            try
            {
                return roslynScriptBroker.GetExportedTypes(
                    assembly: reference)
                    .Any(predicate: type =>
                        imports.Contains(value: type.Namespace));
            }
            catch
            {
                return false;
            }
        });

    private static void SafelyLoadAssembly(
        IRoslynScriptBroker roslynScriptBroker,
        ILoggingBroker loggingBroker,
        ICollection<Assembly> loadedAssemblies,
        string assemblyPath)
    {
        try
        {
            Assembly assembly = roslynScriptBroker.LoadFile(
                path: assemblyPath);

            loadedAssemblies.Add(item: assembly);

            loggingBroker.LogDebug(
                message: "Loaded assembly: {AssemblyName}",
                args: [assembly.FullName]);
        }
        catch (Exception exception)
        {
            loggingBroker.LogWarning(
                message: "Unable to load assembly {AssemblyPath}: {Message}",
                args: [assemblyPath, exception.Message]);
        }
    }
}