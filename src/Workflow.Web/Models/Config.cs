// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Apps.Shared.Models;

public class Config
{
    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    public bool DebugInfo { get; set; }
    public bool LogSQL { get; set; }
}