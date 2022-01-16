﻿using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbCommand
{
    Task<TElement[]> ExecuteQueryAsync<TElement>(string sql);

    Task<byte[]> ExecuteDocumentAsync(OrientDbId id);
}

public class OrientDbCommand : IOrientDbCommand
{
    private readonly IHttpClientFactory clientFactory;
    private readonly IOptionsSnapshot<OrientDbOptions> orientDbOptions;
    private readonly ILogger<OrientDbId> logger;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public OrientDbCommand(IHttpClientFactory clientFactory, IOptionsSnapshot<OrientDbOptions> options, ILogger<OrientDbId> logger)
    {
        this.clientFactory = clientFactory;
        this.orientDbOptions = options;
        this.logger = logger;

        jsonSerializerOptions = new JsonSerializerOptions
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

        var orientDbResult = (await JsonSerializer.DeserializeAsync<OrientDbResult>(stream, jsonSerializerOptions))
            ?? throw new Exception("Unable to parse query result to JsonElement");

        logger.LogExecutionPlan(sql, orientDbResult.ExecutionPlan);

        return orientDbResult.Result.Deserialize<TElement[]>(jsonSerializerOptions) ?? Array.Empty<TElement>();
    }

    public async Task<byte[]> ExecuteDocumentAsync(OrientDbId id)
    {
        var client = CreateDocumentClient();

        var response = await client.GetAsync(id.Value.Replace("#", "").Trim());
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = (await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream))
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return string.IsNullOrWhiteSpace(orientDbResult.Value)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(orientDbResult.Value);
    }

    private HttpClient CreateQueryClient() => CreateOrientDbClient(new Uri($"query/{orientDbOptions.Value.DatabaseName}/sql"));

    private HttpClient CreateDocumentClient() => CreateOrientDbClient(new Uri($"document/{orientDbOptions.Value.DatabaseName}/"));

    private HttpClient CreateOrientDbClient(Uri uri)
    {
        var orientDbUrl = new Uri(orientDbOptions.Value.Url, uri);

        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{orientDbOptions.Value.UserName}:{orientDbOptions.Value.Password}"));
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
        public JsonElement ExecutionPlan { get; set; }

        public JsonElement Result { get; set; }
    }

    private class OrientDbDocumentResult
    {
        public string Value { get; set; }
    }
}