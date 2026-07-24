// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.Workflow.Dependencies.Results;

public class Result
{
    [Key]
    public virtual string Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class Result<T> : Result
{
    private string id;

    [Key]
    public override string Id
    {
        get
        {
            if (id != null)
            {
                return id;
            }

            try
            {
                return Item is null ? null : ((dynamic)Item).Id?.ToString();
            }
            catch
            {
                return null;
            }
        }
        set => id = value;
    }

    public T Item { get; set; }

    public Result<TNew> ToNew<TNew>(TNew item) =>
        new() { Success = Success, Message = Message, Item = item };
}