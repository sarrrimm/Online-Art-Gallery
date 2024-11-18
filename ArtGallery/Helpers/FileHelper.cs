using System;
using System.IO;

namespace ArtGallery.Helpers
{
    public static class FileHelper
    {
        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    // Ensure the file is not read-only
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                    Console.WriteLine($"File deleted successfully: {path}");
                    return true;
                }
                Console.WriteLine($"File not found: {path}");
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the exception (you can use any logging mechanism here)
                Console.WriteLine($"UnauthorizedAccessException: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging mechanism here)
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}
