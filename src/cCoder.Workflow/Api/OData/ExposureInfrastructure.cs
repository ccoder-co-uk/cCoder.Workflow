// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Api.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OData.Edm;


namespace cCoder.Workflow.Api.OData
{
    public static class EdmModelExtensions
    {
        public static ExtendedMetadataContainer GetExtendedMetadataForType(
            this IEdmModel model,
            string context,
            Type type,
            bool hasEndpoint = true
        )
        {
            ExtendedMetadataContainer result = new(type, true, hasEndpoint) { Category = context };
            IEdmEntitySet set = model.EntityContainer.FindEntitySet(type.Name);
            if (set is null)
            {
                result.HasEndpoint = false;
                return result;
            }

            IEnumerable<OperationContainer> customOperations = model
                .FindDeclaredBoundOperations(set.Type)
                .Select(operation => new OperationContainer
                {
                    Name = operation.Name,
                    Url = $"{result.Category}/{type.Name}/{operation.Name}()",
                    Queryable = operation.IsFunction(),
                    HttpVerb = operation.IsFunction() ? "GET" : "POST",
                    ReturnType = BuildMetaFor(operation.GetReturn()?.Type?.Definition),
                    Parameters = operation
                        .Parameters?.Where(parameter => parameter.Name != "bindingParameter")
                        .Select(parameter => new { parameter.Name, TypeName = parameter.Type.FullName() })
                        .ToDictionary(item => item.Name, item => item.TypeName),
                });

            result.Operations = GetBaseCrudOperations(result)
                .Union(customOperations)
                .ToList();

            return result;
        }

        private static MetadataContainer BuildMetaFor(IEdmType definition)
        {
            if (definition?.TypeKind != EdmTypeKind.Collection)
                return null;

            Type cSharpType = Type.GetType(definition.FullTypeName(), false);
            return cSharpType is null ? null : new MetadataContainer(cSharpType, true, true);
        }

        private static IEnumerable<OperationContainer> GetBaseCrudOperations(MetadataContainer type) =>
            type.IsJoinEntity ? GetBaseCrudOperationsForJoinEntity(type) : GetBaseCrudOperationsForEntity(type);

        private static IEnumerable<OperationContainer> GetBaseCrudOperationsForJoinEntity(
            MetadataContainer type
        ) =>
        [
            new()
        {
            Name = "Add",
            Url = $"{type.Category}/{type.Name}",
            Queryable = true,
            HttpVerb = "POST",
            ReturnType = type,
            Parameters = new Dictionary<string, string> { { "body:entity", type.ServerType } },
        },
        new()
        {
            Name = "Get",
            Url = $"{type.Category}/{type.Name}({{Left=leftKey,Right=rightKey}})",
            Queryable = true,
            HttpVerb = "GET",
            ReturnType = type,
            Parameters = new Dictionary<string, string>
            {
                { "odata:key", Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName! },
            },
        },
        new()
        {
            Name = "Get All",
            Url = $"{type.Category}/{type.Name}",
            Queryable = true,
            HttpVerb = "GET",
            ReturnType = type,
        },
        new()
        {
            Name = "Delete",
            Url = $"{type.Category}/{type.Name}({{Left=leftKey,Right=rightKey}})",
            HttpVerb = "DELETE",
        },
        ];

        private static IEnumerable<OperationContainer> GetBaseCrudOperationsForEntity(
            MetadataContainer type
        ) =>
        [
            new()
        {
            Name = "Add",
            Url = $"{type.Category}/{type.Name}",
            Queryable = true,
            HttpVerb = "POST",
            ReturnType = type,
            Parameters = new Dictionary<string, string> { { "body:entity", type.ServerType } },
        },
        new()
        {
            Name = "Update",
            Url = $"{type.Category}/{type.Name}({{key}})",
            Queryable = true,
            HttpVerb = "PUT",
            ReturnType = type,
            Parameters = new Dictionary<string, string>
            {
                { "odata:key", Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName! },
                { "body:entity", type.ServerType },
            },
        },
        new()
        {
            Name = "Get",
            Url = $"{type.Category}/{type.Name}({{key}})",
            Queryable = true,
            HttpVerb = "GET",
            ReturnType = type,
            Parameters = new Dictionary<string, string>
            {
                { "odata:key", Type.GetType(type.ServerType)?.GetIdProperty()?.GetType().FullName! },
            },
        },
        new()
        {
            Name = "Get All",
            Url = $"{type.Category}/{type.Name}",
            Queryable = true,
            HttpVerb = "GET",
            ReturnType = type,
        },
        new() { Name = "Delete", Url = $"{type.Category}/{type.Name}({{key}})", HttpVerb = "DELETE" },
        ];
    }

    public sealed class BadRequestResult : BadRequestObjectResult
    {
        public BadRequestResult(ModelStateDictionary modelState)
            : base(modelState) =>
            Value = modelState
                .Select(item => new ModelStateError
                {
                    Key = item.Key,
                    Value = item.Value?.RawValue,
                    Errors = item.Value?.Errors?.Select(error => $"{error.ErrorMessage} - {error.Exception?.Message}").ToArray(),
                })
                .ToArray()
                .ToJsonForOdata();
    }

    public sealed class ModelStateError
    {
        public string Key { get; set; } = string.Empty;
        public object Value { get; set; }
        public string[] Errors { get; set; }
    }
}