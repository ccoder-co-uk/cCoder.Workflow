// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Models.Results;

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