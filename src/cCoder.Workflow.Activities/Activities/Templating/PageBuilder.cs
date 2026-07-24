// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http.Json;
using cCoder.Data.Models.CMS;

namespace cCoder.Workflow.Activities.Activities.Templating;

public class PageBuilder : TemplatingActivity<dynamic>
{
    public string Title { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string Layout { get; set; }
    public string ResourceKey { get; set; }
    public bool ShowOnMenus { get; set; }
    public int ParentPageId { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();

        PageInfo pageInfo = new()
        {
            CultureId = Culture,
            Description = Description,
            Keywords = Keywords,
            Title = Title
        };

        string renderedHtml = await Render(
            api: api);

        Content content = new()
        {
            CultureId = Culture,
            Name = "body",
            Html = renderedHtml
        };

        Page page = new()
        {
            AppId = AppId,
            Layout = Layout,
            ResourceKey = ResourceKey,
            ShowOnMenus = ShowOnMenus,
            PageInfo = [pageInfo],
            Contents = [content]
        };

        _ = await api.PostAsJsonAsync(
            requestUri: "ContentManagement/Page",
            value: page);
    }
}