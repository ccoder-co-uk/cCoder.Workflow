// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Models;

public class Flow
{
    public string Name { get; set; }

    public string RequiredRoles { get; set; }

    public Activity[] Activities { get; set; }

    public Link[] Links { get; set; }
}