using SQLite;

namespace AcessGallery.Models;

/// <summary>
/// Representa um álbum de fotos criado pelo usuário.
/// </summary>
public class Album
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Nome do álbum.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do álbum.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
