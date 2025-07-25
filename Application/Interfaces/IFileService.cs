﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string webRootPath);
        Task<string> MoveAsync(string filePath, string newFolder, string newFileName = null);
        Task<bool> DeleteFileAsync(string relativeFilePath, string webRootPath);
    }
}
