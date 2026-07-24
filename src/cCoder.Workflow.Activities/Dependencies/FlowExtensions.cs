// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Models;

public static class FlowExtensions
{
    public static T GetActivity<T>(
        this Flow flow,
        string withRef)
            where T : Activity =>
        (T)flow.Activities.FirstOrDefault(
            predicate: activity => activity.Ref == withRef);
}
