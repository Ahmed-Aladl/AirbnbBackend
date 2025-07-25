﻿using Microsoft.AspNetCore.Http;

namespace Application.DTOs.PropertyImageDTOs
{
    public class PropertyImagesUploadContainerDTO
    {
        public int PropertyId { get; set; }
        public string HostId { get; set; }
        public string GroupName { get; set; } = "cover";
        public int CoverIndex { get; set; } // e.g., 0 means first image is cover
        public List<IFormFile> Files { get; set; }
    }

}
