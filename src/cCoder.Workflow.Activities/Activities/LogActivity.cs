// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities;

public abstract class LogActivity : Activity
{

    public string Message { get; set; }

    public override Task ExecuteAsync()
    {
        if (Message != null)
        {
            string level = GetType().Name.Replace(oldValue:"Activity", newValue:"");
            Log(level:(WorkflowLogLevel)Enum.Parse(typeof(WorkflowLogLevel), level), message:Message);
        }
        return base.ExecuteAsync();
    }
}

public class ErrorActivity : LogActivity { }

public class WarningActivity : LogActivity { }

public class InfoActivity : LogActivity { }

public class DebugActivity : LogActivity { }