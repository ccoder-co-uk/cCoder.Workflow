// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Workflow.Activities.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace cCoder.Workflow.Engine.Dependencies;

internal sealed class RoslynScriptDependency
    : IRoslynScriptDependency
{
    private readonly ILogger<RoslynScriptDependency> logger;
    private readonly Assembly[] references;

    public RoslynScriptDependency(
        ILogger<RoslynScriptDependency> logger)
    {
        this.logger = logger;
        references = LoadReferences();
    }

    public async Task<T> BuildScriptAsync<T>(
        string code,
        string[] imports,
        Action<WorkflowLogLevel, string> log)
    {
        try
        {
            ScriptOptions options = BuildScriptOptions(imports: imports);

            return await CSharpScript.EvaluateAsync<T>(
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

    public async Task<T> RunScriptAsync<T>(
        string code,
        string[] imports,
        object args,
        Action<WorkflowLogLevel, string> log)
    {
        try
        {
            IEnumerable<Assembly> requiredReferences =
                ResolveReferences(imports: imports);

            ScriptOptions options = ScriptOptions.Default
                .AddReferences(references: requiredReferences)
                .WithImports(imports: imports);

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

            return (T)await CSharpScript.EvaluateAsync(
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

    private Assembly[] LoadReferences()
    {
        try
        {
            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(predicate: assembly => !assembly.IsDynamic)
                .ToList();

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            string binDirectory = currentAssembly.Location.Replace(
                oldValue: currentAssembly.ManifestModule.Name,
                newValue: string.Empty,
                comparisonType: StringComparison.Ordinal);

            string[] assembliesToLoad = Directory.GetFiles(
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
                    loadedAssemblies: loadedAssemblies,
                    assemblyPath: assemblyPath);
            }

            return loadedAssemblies.ToArray();
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                message: "Script runner may be missing references but will continue: {Message}",
                exception.Message);

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(predicate: assembly => !assembly.IsDynamic)
                .ToArray();
        }
    }

    private ScriptOptions BuildScriptOptions(
        string[] imports) =>
        ScriptOptions.Default
            .AddReferences(
                references: ResolveReferences(imports: imports))
            .WithImports(imports: imports);

    private IEnumerable<Assembly> ResolveReferences(
        string[] imports) =>
        references.Where(predicate: reference =>
        {
            try
            {
                return reference.GetExportedTypes()
                    .Any(predicate: type =>
                        imports.Contains(value: type.Namespace));
            }
            catch
            {
                return false;
            }
        });

    private void SafelyLoadAssembly(
        ICollection<Assembly> loadedAssemblies,
        string assemblyPath)
    {
        try
        {
            Assembly assembly = Assembly.LoadFile(path: assemblyPath);
            loadedAssemblies.Add(item: assembly);

            logger.LogDebug(
                message: "Loaded assembly: {AssemblyName}",
                assembly.FullName);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                message: "Unable to load assembly {AssemblyPath}: {Message}",
                assemblyPath,
                exception.Message);
        }
    }
}