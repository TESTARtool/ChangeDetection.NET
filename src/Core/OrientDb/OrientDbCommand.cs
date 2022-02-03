﻿using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbCommand
{
    Task<TElement[]> ExecuteQueryAsync<TElement>(string sql);

    Task<byte[]> ExecuteDocumentAsync(OrientDbId id);
}

public class OrientDbCommand : IOrientDbCommand
{
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<OrientDbId> logger;
    private readonly OrientDbOptions orientDbOptions;
    private readonly JsonSerializerOptions jsonOptions;

    public OrientDbCommand(IHttpClientFactory clientFactory, IOptions<OrientDbOptions> options, ILogger<OrientDbId> logger)
    {
        this.clientFactory = clientFactory;
        this.logger = logger;
        this.orientDbOptions = options.Value;

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<TElement[]> ExecuteQueryAsync<TElement>(string sql)
    {
        var client = CreateQueryClient();

        var response = await client.GetAsync(sql);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbResult>(stream)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        logger.LogExecutionPlan(sql, orientDbResult.ExecutionPlan);

        return orientDbResult.Result.Deserialize<TElement[]>(jsonOptions) ?? Array.Empty<TElement>();
    }

    public async Task<byte[]> ExecuteDocumentAsync(OrientDbId id)
    {
        var client = CreateDocumentClient();

        var response = await client.GetAsync(id.Value.Replace("#", "").Trim());
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return string.IsNullOrWhiteSpace(orientDbResult.Value)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(orientDbResult.Value);
    }

    private HttpClient CreateQueryClient() => CreateOrientDbClient($"query/{orientDbOptions.DatabaseName}/sql/");

    private HttpClient CreateDocumentClient() => CreateOrientDbClient($"document/{orientDbOptions.DatabaseName}/");

    private HttpClient CreateOrientDbClient(string uri)
    {
        var orientDbUrl = new Uri(orientDbOptions.Url, uri);

        logger.LogInformation($"{orientDbOptions.UserName}:{orientDbOptions.Password}");

        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{orientDbOptions.UserName}:{orientDbOptions.Password}"));
        var client = clientFactory.CreateClient();

        client.BaseAddress = orientDbUrl;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        return client;
    }

    private class OrientDbResult
    {
        [JsonPropertyName("executionPlan")]
        public JsonElement ExecutionPlan { get; set; }

        [JsonPropertyName("result")]
        public JsonElement Result { get; set; }
    }

    private class OrientDbDocumentResult
    {
        public string Value { get; set; }
    }
}