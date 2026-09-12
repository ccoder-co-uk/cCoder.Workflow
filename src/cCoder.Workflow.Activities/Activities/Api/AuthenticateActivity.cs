using System.Text;
using cCoder.Workflow.Activities.Brokers;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Activities.Api;

public class AuthenticateActivity : ApiActivity
{
    record Token(
        string Id,
        int Reason,
        DateTimeOffset Expires,
        string UserName);

    [JsonIgnore]
    public string Username { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        var auth = new { User = Username, Pass = Password };
        HttpResponseMessage response = await api.PostAsync("Account/Login", new StringContent(Json(auth), Encoding.UTF8, "application/json"));
        _ = response.EnsureSuccessStatusCode();
        Token token = await ReadAsAsync<Token>(response.Content);
        AuthToken = token.Id;
    }

    public static async Task<T> ReadAsAsync<T>(HttpContent content)
        => JsonBroker.Deserialize<T>(
            value: await content.ReadAsStringAsync());

    static string Json(object source)
        => JsonBroker.SerializeForOData(value: source);
}