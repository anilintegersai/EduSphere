using EduSphere.Application.Interfaces;

namespace EduSphere.Web.Services;

public sealed class FileSystemAdmissionDocumentStorageService : IAdmissionDocumentStorageService
{
    private readonly string _rootPath;

    public FileSystemAdmissionDocumentStorageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        var configuredRoot = configuration["Storage:AdmissionDocumentsPath"];
        _rootPath = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(environment.ContentRootPath, "App_Data", "Uploads", "Admissions")
            : configuredRoot;

        if (!Path.IsPathRooted(_rootPath))
            _rootPath = Path.Combine(environment.ContentRootPath, _rootPath);

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredAdmissionDocument> SaveAsync(
        Guid tenantId,
        Guid admissionApplicationId,
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = SafeFileName(fileName);
        var extension = Path.GetExtension(safeFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativeDirectory = Path.Combine(tenantId.ToString("N"), admissionApplicationId.ToString("N"));
        var absoluteDirectory = Path.Combine(_rootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var absolutePath = Path.Combine(absoluteDirectory, storedFileName);
        await using (var destination = File.Create(absolutePath))
        {
            await content.CopyToAsync(destination, cancellationToken);
        }

        var relativePath = Path.Combine(relativeDirectory, storedFileName).Replace('\\', '/');
        var length = new FileInfo(absolutePath).Length;
        return new StoredAdmissionDocument(relativePath, safeFileName, contentType, length);
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var normalizedStoragePath = storagePath.Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.GetFullPath(Path.Combine(_rootPath, normalizedStoragePath));
        var root = Path.GetFullPath(_rootPath);
        var relativePath = Path.GetRelativePath(root, absolutePath);

        if (Path.IsPathRooted(relativePath) ||
            relativePath == ".." ||
            relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
            relativePath.StartsWith("../", StringComparison.Ordinal) ||
            !File.Exists(absolutePath))
            throw new FileNotFoundException("Stored admission document was not found.", storagePath);

        return Task.FromResult<Stream>(File.OpenRead(absolutePath));
    }

    private static string SafeFileName(string fileName)
    {
        var safe = string.Join("_", Path.GetFileName(fileName).Split(Path.GetInvalidFileNameChars()));
        return string.IsNullOrWhiteSpace(safe) ? "admission-document" : safe;
    }
}
