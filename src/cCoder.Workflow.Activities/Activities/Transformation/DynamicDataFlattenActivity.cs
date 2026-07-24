// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Activities.Transformation;

public class DynamicDataFlattenActivity : TransformationActivity<IEnumerable<object>, dynamic[]>
{
    public override async Task ExecuteAsync() => Result = await Task.FromResult(result: Source.SelectMany(selector: o => cCoder.Workflow.Activities.Support.Data.Flatten(o))
            .ToArray());
}