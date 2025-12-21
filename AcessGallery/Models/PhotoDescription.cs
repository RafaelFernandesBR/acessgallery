using SQLite;

namespace AcessGallery.Models;

/// <summary>
/// Modelo de dados para armazenar a descrição de uma foto.
/// Utiliza o caminho do arquivo como chave única para vincular a descrição à imagem.
/// </summary>
public class PhotoDescription
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Caminho completo do arquivo de imagem. 
    /// Indexado para buscas rápidas.
    /// </summary>
    [Indexed(Unique = true)]
    public string FilePath { get; set; }

    /// <summary>
    /// Texto alternativo descritivo para acessibilidade.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Data da última modificação da descrição.
    /// </summary>
    public DateTime LastModified { get; set; }
}
