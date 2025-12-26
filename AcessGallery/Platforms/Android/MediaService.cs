using Android.Provider;
using Android.Content;
using AcessGallery.Services;
using Application = Android.App.Application;
using Android.OS;

namespace AcessGallery.Services;

public class MediaService : IMediaService
{
    public async Task<List<string>> GetImagePathsAsync()
    {
        var images = new List<string>();
        
        // Verifica permissões antes de tentar acessar
        var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.StorageRead>();
        }
        
        // Em Android 13+ (API 33), StorageRead pode não ser suficiente para Media, 
        // mas o MAUI lida com granularidade se configurado corretamente.
        // Vamos checar Photos especificamente se necessário
        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
             var statusPhotos = await Permissions.CheckStatusAsync<Permissions.Photos>();
             if (statusPhotos != PermissionStatus.Granted)
             {
                 await Permissions.RequestAsync<Permissions.Photos>();
             }
        }

        if (status != PermissionStatus.Granted && 
            (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Tiramisu))
        {
             // return empty if denied on older android
             return images;
        }

        // Query MediaStore
        var projection = new[] { MediaStore.Images.Media.InterfaceConsts.Data };
        var resolver = Application.Context?.ContentResolver;
        if (resolver == null)
            return images;

        var cursor = resolver.Query(
            MediaStore.Images.Media.ExternalContentUri!,
            projection,
            null,
            null,
            MediaStore.Images.Media.InterfaceConsts.DateAdded + " DESC");

        if (cursor != null)
        {
            while (cursor.MoveToNext())
            {
                var columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                var path = cursor.GetString(columnIndex);
                if (path != null)
                    images.Add(path);
            }
            cursor.Close();
        }

        return images;
    }
}