// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;

namespace cCoder.Workflow.Activities.Activities.Transformation;

public abstract class TransformationActivity<TSource, TResult> : Activity
{

    public TSource Source { get; set; }

    public TResult Result { get; set; }
}