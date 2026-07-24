// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Workflow.Activities;
using cCoder.Workflow.Activities.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace cCoder.Workflow.Engine.Services.Processings;

public sealed class ScriptRunner : IScriptProcessingService
{
    private readonly Assembly[] references;

    public ScriptRunner(LogEvent log)
    {
        try
        {
            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(predicate:assembly => !assembly.IsDynamic)
                .ToList();

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string binDirectory = currentAssembly.Location.Replace(oldValue:currentAssembly.ManifestModule.Name, newValue:string.Empty, comparisonType:StringComparison.Ordinal);

            string[] assembliesToLoad = Directory.GetFiles(path:binDirectory, searchPattern:"*.dll")
                .Where(predicate:path => loadedAssemblies.All(assembly =>
                    !string.Equals(assembly.Location, path, StringComparison.OrdinalIgnoreCase)))
                .Where(predicate:path => !path.Contains("api-ms-win", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string assemblyPath in assembliesToLoad)
                SafelyLoadAssembly(log:log, loadedAssemblies:loadedAssemblies, assemblyPath:assemblyPath);

            references = loadedAssemblies.ToArray();
        }
        catch (Exception exception)
        {
            _ = log(level:WorkflowLogLevel.Warning, message:$"Script runner may be missing references but will continue: {exception.Message}");
            references = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(predicate:assembly => !assembly.IsDynamic)
                .ToArray();
        }
    }

    public async Task<T> BuildScript<T>(string code, string[] imports, Action<WorkflowLogLevel, string> log)
    {
        try
        {
            ScriptOptions options = BuildOptions(imports:imports);
            return await CSharpScript.EvaluateAsync<T>(code:code, options:options);
        }
        catch (Exception exception)
        {
            log(arg1:WorkflowLogLevel.Error, arg2:"Script failed to compile.");
            log(arg1:WorkflowLogLevel.Error, arg2:exception.Message);

            if (exception is Microsoft.CodeAnalysis.Scripting.CompilationErrorException compilationError)
                log(arg1:WorkflowLogLevel.Error, arg2:$"Source of the problem:{Environment.NewLine}{compilationError.Source}");

            return default;
        }
    }

    public async Task<T> Run<T>(
        string code,
        string[] imports,
        object args = null,
        Action<WorkflowLogLevel, string> log = null)
    {
        try
        {
            IEnumerable<Assembly> requiredReferences = ResolveReferences(imports:imports);
            ScriptOptions options = ScriptOptions.Default
                .AddReferences(references:requiredReferences)
                .WithImports(imports:imports);

            if (log is not null)
            {
                string details =
                    $"{Environment.NewLine}Imports{Environment.NewLine}  {string.Join(separator:$"{Environment.NewLine}  ", value:imports)}"
                    + $"{Environment.NewLine}{Environment.NewLine}References Needed{Environment.NewLine}  {string.Join(separator:$"{Environment.NewLine}  ", values:requiredReferences.Select(reference => reference.FullName))}";
                log(arg1:WorkflowLogLevel.Debug, arg2:details);
            }

            return (T)await CSharpScript.EvaluateAsync(code:code, options:options, globals:args, globalsType:args?.GetType());
        }
        catch (NullReferenceException exception)
        {
            string target = exception.TargetSite is null
                ? "unknown"
                : $"(({exception.TargetSite.DeclaringType?.Name ?? "object"})object).{exception.TargetSite.Name}";

            List<string> context = [];

            foreach (object key in exception.Data.Keys)
                context.Add(item:$"{key}: {exception.Data[key]}");

            log?.Invoke(
arg1:                WorkflowLogLevel.Error,
arg2:                $"{exception.Message}{Environment.NewLine}Context: {exception.Source}{Environment.NewLine}Target: {target}{Environment.NewLine}{string.Join(Environment.NewLine, context)}");

            throw;
        }
        catch (Microsoft.CodeAnalysis.Scripting.CompilationErrorException exception)
        {
            log?.Invoke(
arg1:                WorkflowLogLevel.Error,
arg2:                $"Compilation failed:{Environment.NewLine}{exception.Message}{Environment.NewLine}{string.Join(Environment.NewLine, exception.Diagnostics)}");

            throw;
        }
    }

    public Task Run(string code, string[] imports, object args, Action<WorkflowLogLevel, string> log) =>
        Run<bool>(code:$"{code};return true;", imports:imports, args:args, log:log);

    private ScriptOptions BuildOptions(string[] imports) =>
        ScriptOptions.Default
            .AddReferences(references:ResolveReferences(imports))
            .WithImports(imports:imports);

    private IEnumerable<Assembly> ResolveReferences(string[] imports) =>
        references.Where(predicate:reference =>
        {
            try
            {
                return reference.GetExportedTypes().Any(type => imports.Contains(type.Namespace));
            }
            catch
            {
                return false;
            }
        });

    private static void SafelyLoadAssembly(LogEvent log, ICollection<Assembly> loadedAssemblies, string assemblyPath)
    {
        try
        {
            Assembly assembly = Assembly.LoadFile(path:assemblyPath);
            loadedAssemblies.Add(item:assembly);
            _ = log(level:WorkflowLogLevel.Debug, message:$"Loaded assembly: {assembly.FullName}");
        }
        catch (Exception exception)
        {
            _ = log(level:WorkflowLogLevel.Warning, message:$"Unable to load assembly {assemblyPath}: {exception.Message}");
        }
    }
}