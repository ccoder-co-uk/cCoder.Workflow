// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.Results;

public class Result
{
    public virtual string Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class Result<T> : Result
{
    private string id;

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
}