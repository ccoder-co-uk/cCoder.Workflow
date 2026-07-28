// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.Workflow.Models;
using cCoder.Workflow.Models.OData;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Workflow.Brokers.OData;

internal abstract class ODataModelBroker : IODataModelBroker
{
    protected readonly ODataConventionModelBuilder builder;

    protected ODataModelBroker(
        ODataConventionModelBuilder builder = null)
    {
        this.builder =
            builder ?? new ODataConventionModelBuilder();
    }

    public abstract ODataModel Build();

    protected virtual EntitySetConfiguration<T> AddSet<T, TKey>(bool enableBatchingToo = false, string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;
        return builder.EntitySet<T>(name: setName);
    }

    protected virtual EntitySetConfiguration<T> AddJoinSet<T, TKey>(Expression<Func<T, TKey>> key)
        where T : class
    {
        string name = typeof(T).Name;
        EntitySetConfiguration<T> result = builder.EntitySet<T>(name: name);

        builder.EntityType<T>()
            .HasKey(keyDefinitionExpression: key);

        return result;
    }

    protected virtual void AddCommonComplextypes()
    {
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();
    }
}