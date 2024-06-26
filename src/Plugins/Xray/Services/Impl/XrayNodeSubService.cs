﻿namespace Xray.Services.Impl;

public sealed partial class XrayNodeSubService : IXrayNodeSubService
{
    private CancellationTokenSource _cts = new();

    private readonly IHttpClientFactory _httpClientFactory;
    public XrayNodeSubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void Cancel()
    {
        _cts.Cancel();
    }

    public async Task<List<string>> GetXrayNode(string url)
    {
        _cts = new();
        List<string> nodes = [];
        try
        {
            if (url.EndsWith("README.md"))
            {
                nodes = await GetREADME(url);
                return nodes;
            }

            var text = await Request(url);
            return [.. XrayUtils.Base64Decode(text).Split('\n', StringSplitOptions.RemoveEmptyEntries)];
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "获取订阅节点失败");
        }
        return nodes;
    }

    private async Task<List<string>> GetREADME(string url)
    {
        List<string> nodes = [];
        var content = await Request(url);
        if (string.IsNullOrWhiteSpace(content))
        {
            return nodes;
        }
        var reg = NodesRegex().Match(content);
        if (!reg.Success)
        {
            return nodes;
        }
        var text = reg.Result("${nodes}");
        return [.. text.Split('\n', StringSplitOptions.RemoveEmptyEntries)];
    }

    private Task<string> Request(string url)
    {
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(3);
        return http.GetStringAsync(url, _cts.Token);
    }

    [GeneratedRegex("```(?<nodes>[^`]+)```")]
    private static partial Regex NodesRegex();
}
