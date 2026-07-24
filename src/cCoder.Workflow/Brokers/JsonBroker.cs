// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Brokers;

public class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(value: json, settings: CreateJsonSettings());

    public T ParseJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(value: json, settings: CreateJsonSettings());

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(value: value, settings: CreateJsonSettings());

    private static JsonSerializerSettings CreateJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            SerializationBinder = WorkflowCompatibilitySerializationBinder.Instance,
        };

    private sealed class WorkflowCompatibilitySerializationBinder : ISerializationBinder
    {
        internal static readonly ISerializationBinder Instance = new WorkflowCompatibilitySerializationBinder();

        private const string CoreObjectsAssemblyName = "cCoder.Core.Objects";
        private const string CoreConnectivityAssemblyName = "cCoder.Core.Connectivity";
        private const string WorkflowAssemblyName = "cCoder.Workflow.Activities";
        private const string CoreObjectsWorkflowDtoNamespace = "cCoder.Core.Objects.Dtos.Workflow";
        private const string CoreObjectsWorkflowActivityNamespace = "cCoder.Core.Objects.Workflow.Activities";
        private const string WorkflowDtoNamespace = "cCoder.Workflow.Activities";
        private const string WorkflowActivityNamespace = "cCoder.Workflow.Activities";
        private const string WorkflowNestedActivityNamespace = "cCoder.Workflow.Activities.Activities";

        private readonly DefaultSerializationBinder binder = new();

        public Type BindToType(string assemblyName, string typeName)
        {
            foreach ((string candidateAssembly, string candidateType) in GetCandidates(assemblyName: assemblyName, typeName: typeName))
            {
                try
                {
                    Type resolvedType = binder.BindToType(assemblyName: candidateAssembly, typeName: candidateType);
                    if (resolvedType != null)
                    {
                        return resolvedType;
                    }
                }
                catch
                {
                }
            }

            return binder.BindToType(assemblyName: assemblyName, typeName: typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName) =>
            binder.BindToName(serializedType: serializedType, assemblyName: out assemblyName, typeName: out typeName);

        private static IEnumerable<(string assemblyName, string typeName)> GetCandidates(string assemblyName, string typeName)
        {
            if (typeName?.StartsWith(value: CoreObjectsWorkflowDtoNamespace, comparisonType: StringComparison.Ordinal) == true)
            {
                yield return (
                    WorkflowAssemblyName,
                    WorkflowDtoNamespace + typeName[CoreObjectsWorkflowDtoNamespace.Length..]
                );
                yield break;
            }

            if (typeName?.StartsWith(value: CoreObjectsWorkflowActivityNamespace, comparisonType: StringComparison.Ordinal) == true)
            {
                string suffix = typeName[CoreObjectsWorkflowActivityNamespace.Length..];

                yield return (
                    WorkflowAssemblyName,
                    WorkflowActivityNamespace + suffix
                );

                yield return (
                    WorkflowAssemblyName,
                    WorkflowNestedActivityNamespace + suffix
                );

                yield break;
            }

            if (string.Equals(a: assemblyName, b: CoreObjectsAssemblyName, comparisonType: StringComparison.Ordinal) ||
                string.Equals(a: assemblyName, b: CoreConnectivityAssemblyName, comparisonType: StringComparison.Ordinal))
            {
                yield return (WorkflowAssemblyName, typeName);
            }
        }
    }
}