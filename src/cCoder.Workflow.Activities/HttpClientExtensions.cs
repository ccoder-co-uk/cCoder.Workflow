// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;

namespace cCoder.Workflow.Activities.Support;

public static class HttpClientExtensions
{
    private const int BatchSize = 1000;

    public static HttpClient WithBaseUri(this HttpClient client, string baseUriString)
    {
        client.BaseAddress = new Uri(baseUriString);
        return client;
    }

    public static async Task<T> AddAsync<T>(this HttpClient client, string query, T entity)
    {
        HttpResponseMessage response = await client.PostAsync(
requestUri:            query,
content:            new StringContent(entity.ToJsonForOdata(), Encoding.UTF8, mediaType: "application/json"));

        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<T>();
    }

    public static async Task<T> Get<T>(this HttpClient client, string query)
    {
        HttpResponseMessage response = await client.GetAsync(requestUri:query);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<T>();
    }

    public static async Task<T> GetAsync<T>(this HttpClient client, string query)
    {
        HttpResponseMessage response = await client.GetAsync(requestUri:query);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<T>();
    }

    public static async Task<IEnumerable<T>> GetODataCollection<T>(this HttpClient client, string query)
    {
        List<T> results = [];
        int page = 0;
        string fullQuery = query + (query.Contains(value:'?') ? $"&$skip={page * BatchSize}&$top={BatchSize}" : $"?$skip={page * BatchSize}&$top={BatchSize}");

        ODataCollection<T> batch = await client.GetAsync<ODataCollection<T>>(query:fullQuery);

        while (batch?.Value?.Any() ?? false)
        {
            results.AddRange(collection:batch.Value);
            page++;
            fullQuery = query + (query.Contains(value:'?') ? $"&$skip={page * BatchSize}&$top={BatchSize}" : $"?$skip={page * BatchSize}&$top={BatchSize}");
            batch = await client.GetAsync<ODataCollection<T>>(query:fullQuery);
        }

        return results;
    }
}