// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

namespace cCoder.Workflow.Engine.Brokers;

internal sealed class RoslynScriptBroker : IRoslynScriptBroker
{
    public Assembly[] GetCurrentAssemblies() =>
        AppDomain.CurrentDomain.GetAssemblies();

    public Assembly GetExecutingAssembly() =>
        Assembly.GetExecutingAssembly();

    public string[] GetFiles(string path, string searchPattern) =>
        Directory.GetFiles(
            path: path,
            searchPattern: searchPattern);

    public Assembly LoadFile(string path) =>
        Assembly.LoadFile(path: path);

    public Type[] GetExportedTypes(Assembly assembly) =>
        assembly.GetExportedTypes();

    public ScriptOptions BuildOptions(
        IEnumerable<Assembly> references,
        string[] imports) =>
        ScriptOptions.Default
            .AddReferences(references: references)
            .WithImports(imports: imports);

    public Task<T> EvaluateAsync<T>(
        string code,
        ScriptOptions options,
        object globals = null,
        Type globalsType = null) =>
        CSharpScript.EvaluateAsync<T>(
            code: code,
            options: options,
            globals: globals,
            globalsType: globalsType);
}