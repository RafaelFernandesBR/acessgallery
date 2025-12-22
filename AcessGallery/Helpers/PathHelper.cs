namespace AcessGallery.Helpers;

public static class PathHelper
{
    public static string ExtractFileName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "";

        var lastSlash = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
        if (lastSlash >= 0 && lastSlash + 1 < path.Length)
            return path.Substring(lastSlash + 1);

        return path;
    }
}
