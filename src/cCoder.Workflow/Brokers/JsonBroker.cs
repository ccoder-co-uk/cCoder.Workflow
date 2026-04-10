using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace cCoder.Workflow.Brokers;

public interface IJsonBroker
{
    object ParseJson(string json);
    T ParseJson<T>(string json);
    string Serialize(object value);
}

public class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) => JsonConvert.DeserializeObject(json, CreateJsonSettings());

    public T ParseJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, CreateJsonSettings());

    public string Serialize(object value) => JsonConvert.SerializeObject(value, CreateJsonSettings());

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
        private const string CoreConnectivitySftpNamespace = "cCoder.Core.Connectivity.Workflow.Sftp";
        private const string WorkflowDtoNamespace = "cCoder.Workflow.Activities";
        private const string WorkflowActivityNamespace = "cCoder.Workflow.Activities";
        private const string WorkflowNestedActivityNamespace = "cCoder.Workflow.Activities.Activities";
        private const string WorkflowSftpNamespace = "cCoder.Workflow.Activities.Activities.Sftp";

        private readonly DefaultSerializationBinder binder = new();

        public Type BindToType(string assemblyName, string typeName)
        {
            foreach ((string candidateAssembly, string candidateType) in GetCandidates(assemblyName, typeName))
            {
                try
                {
                    Type resolvedType = binder.BindToType(candidateAssembly, candidateType);
                    if (resolvedType != null)
                        return resolvedType;
                }
                catch
                {
                }
            }

            return binder.BindToType(assemblyName, typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName) =>
            binder.BindToName(serializedType, out assemblyName, out typeName);

        private static IEnumerable<(string assemblyName, string typeName)> GetCandidates(string assemblyName, string typeName)
        {
            if (typeName?.StartsWith(CoreObjectsWorkflowDtoNamespace, StringComparison.Ordinal) == true)
            {
                yield return (
                    WorkflowAssemblyName,
                    WorkflowDtoNamespace + typeName[CoreObjectsWorkflowDtoNamespace.Length..]
                );
                yield break;
            }

            if (typeName?.StartsWith(CoreObjectsWorkflowActivityNamespace, StringComparison.Ordinal) == true)
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

            if (typeName?.StartsWith(CoreConnectivitySftpNamespace, StringComparison.Ordinal) == true)
            {
                yield return (
                    WorkflowAssemblyName,
                    WorkflowSftpNamespace + typeName[CoreConnectivitySftpNamespace.Length..]
                );
                yield break;
            }

            if (string.Equals(assemblyName, CoreObjectsAssemblyName, StringComparison.Ordinal) ||
                string.Equals(assemblyName, CoreConnectivityAssemblyName, StringComparison.Ordinal))
            {
                yield return (WorkflowAssemblyName, typeName);
            }
        }
    }
}


