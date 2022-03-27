using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.ConsoleApp.Scenarios;

public class TestMultiCalls : IScenario
{
    private readonly IHttpClientFactory httpClientFactory;

    public TestMultiCalls(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task RunAsync()
    {
        //var testCases = new List<TestCase>
        //{
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="Rick", Password = "Rick" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="testar", Password = "testar" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="Rick", Password = "Rick" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="testar", Password = "testar" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="Rick", Password = "Rick" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="testar", Password = "testar" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="Rick", Password = "Rick" },
        //    new TestCase { Uri = new Uri("http://localhost:5001"), Username ="testar", Password = "testar" },
        //};

        var testCases = new List<TestCase>
        {
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\Rick", Password = "Rick" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\Rick", Password = "Rick" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar2\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar2\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar2\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar2\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar2\\testar", Password = "testar" },
            new TestCase { Uri = new Uri("http://localhost:5000"), Username ="testar\\testar", Password = "testar" },
        };

        Console.Write("Running Test ");

        var testTasks = testCases.Select(x => RunTest(x))
            .ToList();

        await Task.WhenAll(testTasks);

        //foreach (var test in testCases)
        //{
        //    await RunTest(test);
        //}

        Console.WriteLine("[DONE]");

        Console.WriteLine("Test Results");
        foreach (var test in testCases)
        {
            Console.WriteLine($"Uri : {test.Uri}");
            Console.WriteLine($"    UserN : {test.Username}");
            Console.WriteLine($"    Passw : {test.Password}");
            Console.WriteLine($"    Login : {test.StatusCodeLogin}");
            Console.WriteLine($"    Query : {test.StatusCodeQuery?.ToString() ?? "XX"}");
            Console.WriteLine($"    oDbID : {test.SessionId?.ToString() ?? "XXXXX"}");
        }
    }

    private async Task RunTest(TestCase test)
    {
        var loginClient = CreateHttpClient(test.Uri, null);

        var loginAsJson = JsonSerializer.Serialize(new LoginModel
        {
            Username = test.Username,
            Password = test.Password,
        });

        var loginResponse = await loginClient.PostAsync($"/api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

        test.StatusCodeLogin = loginResponse.StatusCode;

        if (loginResponse.IsSuccessStatusCode)
        {
            var token = await loginResponse.Content.ReadAsStringAsync();
            var httpClient = CreateHttpClient(test.Uri, token);

            test.SessionId = ParseClaimsFromJwt(token).First(x => x.Type == "OrientDbSession").Value;
            var url = $"/api/query";

            var json = JsonSerializer.Serialize(new OrientDbCommand("SELECT FROM AbstractStateModel"));
            using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(httpContent);
            test.StatusCodeQuery = response.StatusCode;
        }
    }

    private HttpClient CreateHttpClient(Uri uri, string? token)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.BaseAddress = uri;

        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return httpClient;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? throw new Exception("keyvalue pairs is null");
        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")));

        return claims;
    }

    public class TestCase
    {
        public Uri Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public HttpStatusCode? StatusCodeLogin { get; set; }
        public HttpStatusCode? StatusCodeQuery { get; set; }
        public string? SessionId { get; set; }
    }
}