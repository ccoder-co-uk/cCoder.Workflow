// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Activities.Support;

public class Path
{
    private static readonly Path empty = new(
        path: string.Empty);

    public static Path Empty => empty;

    public string Name => Segments.LastOrDefault();

    public string FullPath { get; }

    public string Lowered => FullPath.ToLowerInvariant();

    public string[] Segments => FullPath.Split(separator: '/');

    public Path ParentPath =>
        Segments.Length > 1
            ? new(
                path: string
                    .Join(
                        separator: "/",
                        value: Segments)[..(FullPath.Length - (1 + Segments
                            .Last()
                            .Length))])
            : Empty;

    public string Extension =>
        Segments.LastOrDefault()?.Contains(value: '.') ?? false
            ? Segments
                .Last()
                .Split(
                    separator: '.')
                .Last()
                .ToLowerInvariant()
            : string.Empty;

    public int Length => FullPath.Length;

    public int Depth => Segments.Length;

    public bool IsToFile => Extension.Length > 0;

    public Path(string path)
    {
        FullPath = (path ?? string.Empty)
            .Trim()
            .TrimEnd(
                trimChar: '/');
    }

    public override string ToString() =>
        FullPath;
}