using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async Task<string> UploadFileAsync(IFormFile file, string webRootPath)
        {
            var uploadsPath = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueName}";
        }


        public async Task<string> MoveAsync(string filePath, string newFolder, string newFileName=null)
        {
            // Source file path in wwwroot/uploads
            var sourceFilePath = filePath;

            // Check if source file exists
            if (!File.Exists(sourceFilePath))
            {
                return "not found";
            }

            // Create private folder if it doesn't exist
            if (!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }
            if(newFileName == null)
                newFileName = Path.GetFileName(filePath);
            // Destination file path in private folder (keeping same name)
            var destinationFilePath = Path.Combine(newFolder, newFileName );

            // Move the file (this is more efficient than copy + delete)
            File.Move(sourceFilePath, destinationFilePath);

            // Return the new file path
            return destinationFilePath;
        }

        public async Task<bool> DeleteFileAsync(string relativeFilePath, string webRootPath)
        {
            try
            {
                // Ensure path is clean and rooted in wwwroot
                var trimmedPath = relativeFilePath
                    .TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(webRootPath, trimmedPath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false; // File doesn't exist
            }
            catch
            {
                // Optionally log the exception here
                return false;
            }
        }
    }
}
