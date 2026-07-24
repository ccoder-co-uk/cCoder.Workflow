// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace cCoder.Workflow.Activities.Activities.Api;

public class AuthenticateActivity : ApiActivity
{
    private sealed class Token
    {
        public string Id { get; set; }

        public int Reason { get; set; }

        public DateTimeOffset Expires { get; set; }

        public string UserName { get; set; }
    }

    [JsonIgnore]
    public string Username { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        var auth = new { User = Username, Pass = Password };
        HttpResponseMessage response = await api.PostAsync(requestUri: "Account/Login", content: new StringContent(Json(source: auth), Encoding.UTF8, "application/json"));
        _ = response.EnsureSuccessStatusCode();
        Token token = await ReadAsAsync<Token>(content: response.Content);
        AuthToken = token.Id;
    }

    public static async Task<T> ReadAsAsync<T>(HttpContent content)
        => JsonConvert.DeserializeObject<T>(value: await content.ReadAsStringAsync());

    static string Json(object source)
        => JsonConvert.SerializeObject(value: source, settings: new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            MaxDepth = 4
        });
}