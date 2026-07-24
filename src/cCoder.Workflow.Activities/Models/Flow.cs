// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Workflow.Activities.Activities;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Models;

public class Flow
{
    [Key]
    [Required]
    public string Name { get; set; }

    public string RequiredRoles { get; set; }

    public Activity[] Activities { get; set; }

    public Link[] Links { get; set; }

    public T GetActivity<T>(string withRef) where T : Activity => (T)Activities.FirstOrDefault(predicate:a => a.Ref == withRef);
}