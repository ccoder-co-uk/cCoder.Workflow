// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Apps.Shared.Models;

public class Config
{
    public IDictionary<string, string> ConnectionStrings { get; set; }
    public IDictionary<string, string> Settings { get; set; }
    public IDictionary<string, string> Services { get; set; }
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
}