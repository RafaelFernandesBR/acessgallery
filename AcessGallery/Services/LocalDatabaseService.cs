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
        await _database.CreateTableAsync<Album>();
        await _database.CreateTableAsync<AlbumPhoto>();
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

    #region Albums

    public async Task<List<Album>> GetAlbumsAsync()
    {
        await InitAsync();
        return await _database!.Table<Album>().OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task CreateAlbumAsync(Album album)
    {
        await InitAsync();
        await _database!.InsertAsync(album);
    }

    public async Task DeleteAlbumAsync(Album album)
    {
        await InitAsync();
        // Remove photos from album first (cascade delete logic manually)
        var photos = await _database!.Table<AlbumPhoto>().Where(x => x.AlbumId == album.Id).ToListAsync();
        foreach (var p in photos)
        {
            await _database.DeleteAsync(p);
        }
        await _database.DeleteAsync(album);
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        await InitAsync();
        await _database!.UpdateAsync(album);
    }

    public async Task AddPhotoToAlbumAsync(int albumId, string filePath)
    {
        await InitAsync();
        
        var hash = await FileHashService.ComputeHashAsync(filePath);
        
        // Verifica se já existe no álbum
        var existing = await _database!.Table<AlbumPhoto>()
            .Where(x => x.AlbumId == albumId && x.PhotoHash == hash)
            .FirstOrDefaultAsync();

        if (existing == null)
        {
            var ap = new AlbumPhoto
            {
                AlbumId = albumId,
                FilePath = filePath,
                PhotoHash = hash,
                AddedAt = DateTime.Now
            };
            await _database.InsertAsync(ap);
        }
    }

    public async Task RemovePhotoFromAlbumAsync(int albumId, string filePath)
    {
        await InitAsync();
        var hash = await FileHashService.ComputeHashAsync(filePath);
        
        var existing = await _database!.Table<AlbumPhoto>()
            .Where(x => x.AlbumId == albumId && x.PhotoHash == hash)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            await _database.DeleteAsync(existing);
        }
    }

    public async Task<List<AlbumPhoto>> GetPhotosByAlbumIdAsync(int albumId)
    {
        await InitAsync();
        return await _database!.Table<AlbumPhoto>()
            .Where(x => x.AlbumId == albumId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync();
    }

    #endregion

    #region Backup

    public async Task BackupDatabaseAsync()
    {
        // Garante que iniciou para saber o caminho, mas fecha antes de copiar
        if (_database == null) await InitAsync();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);
        
        // Fecha conexão para garantir integridade (checkpoint do WAL, release lock)
        await _database.CloseAsync();
        _database = null;

        if (!File.Exists(dbPath))
            return;

        var backupPath = Path.Combine(FileSystem.CacheDirectory, $"backup_acessgallery_{DateTime.Now:yyyyMMdd_HHmm}.db3");

        File.Copy(dbPath, backupPath, true);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Backup do Banco de Dados AcessGallery",
            File = new ShareFile(backupPath)
        });
    }

    public async Task<bool> RestoreDatabaseAsync(string backupFilePath)
    {
        try
        {
            if (_database != null)
            {
                await _database.CloseAsync();
                _database = null;
            }

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbName);

            File.Copy(backupFilePath, dbPath, true);

            await InitAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error restoring backup: {ex.Message}");
            return false;
        }
    }

    #endregion
}
