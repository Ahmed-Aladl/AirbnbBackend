namespace Airbnb.DTOs.PropertyImageDTOs
{
    public class PropertyImagesUploadContainerDTO
    {
        public int PropertyId { get; set; }
        public string HostId { get; set; }
        public string GroupName { get; set; }
        public int CoverIndex { get; set; } // e.g., 0 means first image is cover
        public List<IFormFile> Files { get; set; } 
    }

}
