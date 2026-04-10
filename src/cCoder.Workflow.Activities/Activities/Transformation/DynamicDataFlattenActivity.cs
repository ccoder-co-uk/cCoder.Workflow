namespace cCoder.Workflow.Activities.Activities.Transformation;

public class DynamicDataFlattenActivity : TransformationActivity<IEnumerable<object>, dynamic[]>
{
    public override async Task ExecuteAsync() => Result = await Task.FromResult(Source.SelectMany(o => cCoder.Workflow.Activities.Support.Data.Flatten(o)).ToArray());
}






