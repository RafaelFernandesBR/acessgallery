using SQLite;

namespace AcessGallery.Models;

/// <summary>
/// Tabela de junção para associar fotos a álbuns.
/// </summary>
public class AlbumPhoto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// ID do álbum ao qual a foto pertence.
    /// </summary>
    [Indexed]
    public int AlbumId { get; set; }

    /// <summary>
    /// Caminho do arquivo da foto.
    /// </summary>
    [Indexed]
    public string? FilePath { get; set; }

    /// <summary>
    /// Hash da foto para manter a referência mesmo se o caminho mudar.
    /// </summary>
    [Indexed]
    public string? PhotoHash { get; set; }

    /// <summary>
    /// Data em que a foto foi adicionada ao álbum.
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.Now;
}
