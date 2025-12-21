using SQLite;
using AcessGallery.Models;

namespace AcessGallery.Services;

/// <summary>
/// Serviço responsável pelo armazenamento local das descrições das fotos usando SQLite.
/// </summary>
public class LocalDatabaseService
{
    private SQLiteAsyncConnection? _database;
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
        // Tenta obter o hash do próprio arquivo e buscar pela hash
        var hash = await FileHashService.ComputeHashAsync(filePath);
        if (!string.IsNullOrEmpty(hash))
        {
            var byHash = await _database!.Table<PhotoDescription>()
                .Where(x => x.Hash == hash)
                .FirstOrDefaultAsync();

            if (byHash != null)
            {
                // Atualiza o FilePath caso tenha mudado
                if (byHash.FilePath != filePath)
                {
                    byHash.FilePath = filePath;
                    await _database.UpdateAsync(byHash);
                }
                return byHash;
            }
        }

        // Fallback: busca pelo caminho (compatibilidade)
        return await _database!.Table<PhotoDescription>()
            .Where(x => x.FilePath == filePath)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Salva ou atualiza a descrição de uma foto.
    /// </summary>
    public async Task SaveDescriptionAsync(PhotoDescription item)
    {
        await InitAsync();
        // Calcula hash do próprio arquivo e usa como chave primaria estável
        var hash = await FileHashService.ComputeHashAsync(item.FilePath ?? string.Empty);
        item.Hash = hash;

        PhotoDescription? existing = null;
        if (!string.IsNullOrEmpty(hash))
        {
            existing = await _database!.Table<PhotoDescription>()
                .Where(x => x.Hash == hash)
                .FirstOrDefaultAsync();
        }

        // Se não encontrou por hash, tenta por caminho (compatibilidade)
        if (existing == null)
            existing = await _database!.Table<PhotoDescription>()
                .Where(x => x.FilePath == item.FilePath)
                .FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.Description = item.Description;
            existing.LastModified = DateTime.Now;
            existing.FilePath = item.FilePath;
            existing.Hash = item.Hash;
            await _database!.UpdateAsync(existing);
        }
        else
        {
            item.LastModified = DateTime.Now;
            await _database!.InsertAsync(item);
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

        var q = query.ToLower();
        return await _database!.Table<PhotoDescription>()
            .Where(x => x.Description != null && x.Description.ToLower().Contains(q))
            .ToListAsync();
    }
}
