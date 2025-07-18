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
        public async Task<bool> DeleteFileAsync(string relativeFilePath, string webRootPath)
        {
            try
            {
                // Ensure path is clean and rooted in wwwroot
                var trimmedPath = relativeFilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
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
