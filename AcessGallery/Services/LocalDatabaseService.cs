using SQLite;
using AcessGallery.Models;

namespace AcessGallery.Services;

/// <summary>
/// Serviço responsável pelo armazenamento local das descrições das fotos usando SQLite.
/// </summary>
public class LocalDatabaseService
{
    private SQLiteAsyncConnection _database;
    private const string DbName = "PhotoDescriptions.db3";

    public LocalDatabaseService()
    {
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<PhotoDescription>();
    }

    /// <summary>
    /// Recupera a descrição salva para um caminho de imagem específico.
    /// </summary>
    public async Task<PhotoDescription> GetDescriptionAsync(string filePath)
    {
        await InitAsync();
        return await _database.Table<PhotoDescription>()
            .Where(x => x.FilePath == filePath)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Salva ou atualiza a descrição de uma foto.
    /// </summary>
    public async Task SaveDescriptionAsync(PhotoDescription item)
    {
        await InitAsync();
        
        // Verifica se já existe
        var existing = await GetDescriptionAsync(item.FilePath);
        
        if (existing != null)
        {
            existing.Description = item.Description;
            existing.LastModified = DateTime.Now;
            await _database.UpdateAsync(existing);
        }
        else
        {
            item.LastModified = DateTime.Now;
            await _database.InsertAsync(item);
        }
    }
    
    /// <summary>
    /// Busca descrições que contêm o texto informado.
    /// </summary>
    public async Task<List<PhotoDescription>> SearchDescriptionsAsync(string query)
    {
        await InitAsync();
        if (string.IsNullOrWhiteSpace(query))
            return new List<PhotoDescription>();

        return await _database.Table<PhotoDescription>()
            .Where(x => x.Description.ToLower().Contains(query.ToLower()))
            .ToListAsync();
    }
}
