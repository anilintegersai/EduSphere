using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EduSphere.Application.DTOs.AI;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Enums;

namespace EduSphere.Infrastructure.AI;

public abstract class OpenAICompatibleQuestionPaperProvider : IAIQuestionPaperProvider
{
    private static readonly HttpClient HttpClient = new();
    public abstract AIProviderType ProviderType { get; }

    public async Task<AIProviderGenerationResult> GenerateAsync(AIProviderGenerationRequest request, CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(request.TimeoutSeconds, 10, 300)));
        using var message = new HttpRequestMessage(HttpMethod.Post, BuildUri(request));
        ConfigureAuthentication(message, request.ApiKey);
        message.Content = JsonContent.Create(new
        {
            model = request.ModelOrDeployment,
            temperature = 0.2,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            }
        });

        var stopwatch = Stopwatch.StartNew();
        using var response = await HttpClient.SendAsync(message, timeout.Token);
        var body = await response.Content.ReadAsStringAsync(timeout.Token);
        stopwatch.Stop();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"AI provider returned {(int)response.StatusCode}: {SafeProviderError(body)}");

        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        var content = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
            ?? throw new InvalidOperationException("AI provider response did not contain message content.");
        var inputTokens = root.TryGetProperty("usage", out var usage) && usage.TryGetProperty("prompt_tokens", out var input) ? input.GetInt32() : 0;
        var outputTokens = root.TryGetProperty("usage", out usage) && usage.TryGetProperty("completion_tokens", out var output) ? output.GetInt32() : 0;
        var requestId = response.Headers.TryGetValues("x-request-id", out var values) ? values.FirstOrDefault() : null;
        var model = root.TryGetProperty("model", out var modelElement) ? modelElement.GetString() ?? request.ModelOrDeployment : request.ModelOrDeployment;
        return new AIProviderGenerationResult(content, inputTokens, outputTokens, (int)stopwatch.ElapsedMilliseconds, requestId, model);
    }

    protected abstract Uri BuildUri(AIProviderGenerationRequest request);
    protected abstract void ConfigureAuthentication(HttpRequestMessage message, string apiKey);

    private static string SafeProviderError(string body) => body.Length <= 500 ? body : body[..500];
}

public sealed class OpenAIQuestionPaperProvider : OpenAICompatibleQuestionPaperProvider
{
    public override AIProviderType ProviderType => AIProviderType.OpenAI;
    protected override Uri BuildUri(AIProviderGenerationRequest request)
    {
        var endpoint = request.Endpoint.TrimEnd('/');
        if (endpoint.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase)) return new Uri(endpoint);
        return new Uri(endpoint + (endpoint.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ? "/chat/completions" : "/v1/chat/completions"));
    }
    protected override void ConfigureAuthentication(HttpRequestMessage message, string apiKey) =>
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
}

public sealed class AzureOpenAIQuestionPaperProvider : OpenAICompatibleQuestionPaperProvider
{
    public override AIProviderType ProviderType => AIProviderType.AzureOpenAI;
    protected override Uri BuildUri(AIProviderGenerationRequest request)
    {
        var endpoint = request.Endpoint.TrimEnd('/');
        if (endpoint.Contains("/chat/completions", StringComparison.OrdinalIgnoreCase)) return new Uri(endpoint);
        var version = string.IsNullOrWhiteSpace(request.ApiVersion) ? "2024-10-21" : request.ApiVersion;
        return new Uri($"{endpoint}/openai/deployments/{Uri.EscapeDataString(request.ModelOrDeployment)}/chat/completions?api-version={Uri.EscapeDataString(version)}");
    }
    protected override void ConfigureAuthentication(HttpRequestMessage message, string apiKey) => message.Headers.Add("api-key", apiKey);
}
