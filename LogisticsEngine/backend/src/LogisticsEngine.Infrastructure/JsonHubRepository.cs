using System.Text.Json;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure;

/// <summary>Repository JSON sin asincronia: adatto alla console didattica, non a carichi concorrenti elevati.</summary>
public sealed class JsonHubRepository(string dataDirectory) : IHubRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.GetFullPath(dataDirectory);

    public Hub? GetById(Guid id) => Read(FilePath(id));
    public IReadOnlyList<Hub> GetAll()
    {
        if (!Directory.Exists(_directory))
        {
            return [];
        }
        return Directory.EnumerateFiles(_directory, "*.json")
            .Select(Read).Where(hub => hub is not null).Select(hub => hub!)
            .OrderBy(hub => hub.Name, StringComparer.Ordinal).ToArray();
    }
    public void Add(Hub hub) => Write(hub, overwrite: false);
    public bool Update(Hub hub)
    {
        if (!File.Exists(FilePath(hub.Id)))
        {
            return false;
        }
        Write(hub, overwrite: true);
        return true;
    }
    public bool Delete(Guid id)
    {
        string path = FilePath(id);
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    // Adapter asincroni temporanei per i client legacy della Web API; l'Application usa solo il contratto sincrono.
    public Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(GetById(id));
    public Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(GetAll());
    public Task AddAsync(Hub hub, CancellationToken cancellationToken = default) { Add(hub); return Task.CompletedTask; }
    public Task<bool> UpdateAsync(Hub hub, CancellationToken cancellationToken = default) => Task.FromResult(Update(hub));
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Delete(id));

    private string FilePath(Guid id) => Path.Combine(_directory, $"{id:D}.json");
    private static Hub? Read(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        HubDocument document = JsonSerializer.Deserialize<HubDocument>(File.ReadAllText(path), _jsonOptions)
            ?? throw new InvalidDataException($"Empty hub document: {path}");
        Result<Hub> result = Hub.Create(document.Id, document.Name, document.Zone);
        if (result.IsFailure || Path.GetFileNameWithoutExtension(path) != document.Id.ToString("D"))
        {
            throw new InvalidDataException($"Invalid hub document: {path}");
        }
        return result.Value;
    }
    private void Write(Hub hub, bool overwrite)
    {
        Directory.CreateDirectory(_directory);
        string path = FilePath(hub.Id);
        string temporaryPath = path + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(new HubDocument(hub.Id, hub.Name, hub.Zone.Code), _jsonOptions));
            File.Move(temporaryPath, path, overwrite);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
    private sealed record HubDocument(Guid Id, string Name, string Zone);
}
