using System.Net.Http.Headers;
using System.Security.Cryptography;
using EduSphere.Domain.Enums;

namespace EduSphere.Web.Services;

public sealed record DocumentScanResult(DocumentScanStatus Status, string Message, string Sha256);

public interface IDocumentVirusScanner
{
    Task<DocumentScanResult> ScanAsync(Stream content, CancellationToken cancellationToken = default);
}

public interface IProfileDocumentStorage
{
    string ProviderName { get; }
    Task<string> SaveAsync(string key, Stream content, string? contentType, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string key, CancellationToken cancellationToken = default);
}

public sealed class BuiltInDocumentVirusScanner : IDocumentVirusScanner
{
    private static readonly byte[] Eicar = "X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*"u8.ToArray();

    public async Task<DocumentScanResult> ScanAsync(Stream content, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();
        content.Position = 0;
        var hash = Convert.ToHexString(SHA256.HashData(bytes));

        var infected = bytes.AsSpan().IndexOf(Eicar) >= 0 ||
                       (bytes.Length > 2 && bytes[0] == (byte)'M' && bytes[1] == (byte)'Z');
        return infected
            ? new(DocumentScanStatus.Infected, "The upload matched a blocked executable or antivirus test signature.", hash)
            : new(DocumentScanStatus.Clean, "Built-in signature scan completed.", hash);
    }
}

public sealed class ConfigurableProfileDocumentStorage : IProfileDocumentStorage
{
    private readonly string _provider;
    private readonly string _localRoot;
    private readonly string? _baseUrl;
    private readonly string? _apiKey;
    private readonly IHttpClientFactory _clients;

    public ConfigurableProfileDocumentStorage(IConfiguration configuration, IWebHostEnvironment environment, IHttpClientFactory clients)
    {
        _provider = configuration["ProfileDocuments:Storage:Provider"] ?? "Local";
        var configuredRoot = configuration["ProfileDocuments:Storage:LocalRoot"];
        _localRoot = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(environment.ContentRootPath, "App_Data", "ProfileDocuments")
            : configuredRoot;
        _baseUrl = configuration["ProfileDocuments:Storage:External:BaseUrl"];
        _apiKey = configuration["ProfileDocuments:Storage:External:ApiKey"];
        _clients = clients;
    }

    public string ProviderName => _provider;

    public async Task<string> SaveAsync(string key, Stream content, string? contentType, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(_provider, "ExternalHttp", StringComparison.OrdinalIgnoreCase))
        {
            var path = SafeLocalPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var target = File.Create(path);
            await content.CopyToAsync(target, cancellationToken);
            return key;
        }

        if (string.IsNullOrWhiteSpace(_baseUrl))
            throw new InvalidOperationException("External document storage requires ProfileDocuments:Storage:External:BaseUrl.");

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl.TrimEnd('/')}/{Uri.EscapeDataString(key)}");
        request.Content = new StreamContent(content);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/octet-stream");
        if (!string.IsNullOrWhiteSpace(_apiKey)) request.Headers.Add("X-Api-Key", _apiKey);
        using var response = await _clients.CreateClient().SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return key;
    }

    public async Task<Stream?> OpenReadAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(_provider, "ExternalHttp", StringComparison.OrdinalIgnoreCase))
        {
            var path = SafeLocalPath(key);
            return File.Exists(path) ? File.OpenRead(path) : null;
        }

        if (string.IsNullOrWhiteSpace(_baseUrl)) return null;
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl.TrimEnd('/')}/{Uri.EscapeDataString(key)}");
        if (!string.IsNullOrWhiteSpace(_apiKey)) request.Headers.Add("X-Api-Key", _apiKey);
        var response = await _clients.CreateClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode) { response.Dispose(); return null; }
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return new ResponseOwnedStream(stream, response);
    }

    private string SafeLocalPath(string key)
    {
        var root = Path.GetFullPath(_localRoot);
        var normalized = key.Replace('\\', '/');
        const string legacyPrefix = "App_Data/ProfileDocuments/";
        if (normalized.StartsWith(legacyPrefix, StringComparison.OrdinalIgnoreCase))
            normalized = normalized[legacyPrefix.Length..];
        var path = Path.GetFullPath(Path.Combine(root, normalized.Replace('/', Path.DirectorySeparatorChar)));
        if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid document storage key.");
        return path;
    }

    private sealed class ResponseOwnedStream(Stream inner, IDisposable owner) : Stream
    {
        public override bool CanRead => inner.CanRead; public override bool CanSeek => inner.CanSeek; public override bool CanWrite => false;
        public override long Length => inner.Length; public override long Position { get => inner.Position; set => inner.Position = value; }
        public override void Flush() => inner.Flush(); public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin); public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => inner.ReadAsync(buffer, offset, count, cancellationToken);
        protected override void Dispose(bool disposing) { if (disposing) { inner.Dispose(); owner.Dispose(); } base.Dispose(disposing); }
    }
}
