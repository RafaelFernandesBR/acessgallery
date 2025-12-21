namespace AcessGallery.Services;

public interface IMediaService
{
    Task<List<string>> GetImagePathsAsync();
}
