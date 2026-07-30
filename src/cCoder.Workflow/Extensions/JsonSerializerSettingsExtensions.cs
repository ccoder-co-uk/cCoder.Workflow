// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Extensions;

internal static class JsonSerializerSettingsExtensions
{
    internal static JsonSerializerSettings ConfigureForWorkflow(
        this JsonSerializerSettings settings)
    {
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.TypeNameHandling = TypeNameHandling.Objects;
        settings.Formatting = Formatting.None;
        settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;


        settings.ContractResolver =
            new DefaultContractResolver
            {
                IgnoreSerializableAttribute = true
            };


        settings.SerializationBinder =
            WorkflowCompatibilitySerializationBinder.Instance;

        return settings;
    }

    private sealed class WorkflowCompatibilitySerializationBinder
        : ISerializationBinder
    {
        private const string CoreObjectsAssemblyName =
            "cCoder.Core.Objects";
        private const string CoreConnectivityAssemblyName =
            "cCoder.Core.Connectivity";
        private const string WorkflowAssemblyName =
            "cCoder.Workflow.Activities";
        private const string CoreObjectsWorkflowDtoNamespace =
            "cCoder.Core.Objects.Dtos.Workflow";
        private const string CoreObjectsWorkflowActivityNamespace =
            "cCoder.Core.Objects.Workflow.Activities";
        private const string WorkflowDtoNamespace =
            "cCoder.Workflow.Activities";
        private const string WorkflowActivityNamespace =
            "cCoder.Workflow.Activities";
        private const string WorkflowNestedActivityNamespace =
            "cCoder.Workflow.Activities.Activities";

        internal static ISerializationBinder Instance { get; } =
            new WorkflowCompatibilitySerializationBinder();

        private readonly DefaultSerializationBinder binder = new();

        public Type BindToType(
            string assemblyName,
            string typeName)
        {
            foreach ((string candidateAssembly, string candidateType)
                in GetCandidates(
                    assemblyName: assemblyName,
                    typeName: typeName))
            {
                try
                {
                    Type resolvedType = binder.BindToType(
                        assemblyName: candidateAssembly,
                        typeName: candidateType);

                    if (resolvedType != null)
                    {
                        return resolvedType;
                    }
                }
                catch
                {
                }
            }

            return binder.BindToType(
                assemblyName: assemblyName,
                typeName: typeName);
        }

        public void BindToName(
            Type serializedType,
            out string assemblyName,
            out string typeName) =>
            binder.BindToName(
                serializedType: serializedType,
                assemblyName: out assemblyName,
                typeName: out typeName);

        private static IEnumerable<(string assemblyName, string typeName)>
            GetCandidates(
                string assemblyName,
                string typeName)
        {
            if (typeName?.StartsWith(
                value: CoreObjectsWorkflowDtoNamespace,
                comparisonType: StringComparison.Ordinal) == true)
            {
                yield return (
                    WorkflowAssemblyName,
                    WorkflowDtoNamespace
                        + typeName[CoreObjectsWorkflowDtoNamespace.Length..]);

                yield break;
            }

            if (typeName?.StartsWith(
                value: CoreObjectsWorkflowActivityNamespace,
                comparisonType: StringComparison.Ordinal) == true)
            {
                string suffix =
                    typeName[CoreObjectsWorkflowActivityNamespace.Length..];

                yield return (
                    WorkflowAssemblyName,
                    WorkflowActivityNamespace + suffix);

                yield return (
                    WorkflowAssemblyName,
                    WorkflowNestedActivityNamespace + suffix);

                yield break;
            }

            if (string.Equals(
                a: assemblyName,
                b: CoreObjectsAssemblyName,
                comparisonType: StringComparison.Ordinal)
                || string.Equals(
                    a: assemblyName,
                    b: CoreConnectivityAssemblyName,
                    comparisonType: StringComparison.Ordinal))
            {
                yield return (
                    WorkflowAssemblyName,
                    typeName);
            }
        }
    }
}