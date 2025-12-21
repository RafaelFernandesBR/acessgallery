using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AcessGallery.Services
{
    public static class FileHashService
    {
        public static async Task<string?> ComputeHashAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                if (!File.Exists(filePath))
                    return null;

                return await Task.Run(() =>
                {
                    using var stream = File.OpenRead(filePath);
                    using var sha = SHA256.Create();
                    var hashBytes = sha.ComputeHash(stream);
                    var sb = new StringBuilder(hashBytes.Length * 2);
                    foreach (var b in hashBytes)
                        sb.Append(b.ToString("x2"));

                    return sb.ToString();
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
