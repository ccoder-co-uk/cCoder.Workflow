// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Activities.Transformation;

public class ConvertActivity<TSource, TResult> : TransformationActivity<IEnumerable<TSource>, TResult[]>
{

    public IEnumerable<string> Expressions { get; set; }

    public class ScriptArgs<T>
    {
        public IEnumerable<T> Source { get; set; }
    }

    public override async Task ExecuteAsync() =>
        Result = (await ExecuteScript<IEnumerable<TResult>>(code:BuildFunctionCode(), args:new ScriptArgs<TSource> { Source = Source })).ToArray();

    private string BuildFunctionCode()
    {
        string assigns = string.Join(separator:",\n\t\t\t", value:Expressions?.ToArray() ?? System.Array.Empty<string>()).Replace(oldValue:"{source}", newValue:"item").Replace(oldValue:"\n", newValue:"\n\t\t");
        return @"   Source.Select((" + typeof(TSource).Name == "object" ? "dynamic" : typeof(TSource).Name + @" item) => {
        return new " + typeof(TResult).Name + @"() 
        { 
            " + assigns + @" 
        };
    })";
    }
}