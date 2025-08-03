using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyImageDTOs;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Result;
using Application.Shared;
using AutoMapper;
using AutoMapper.Features;
using AutoMapper.Internal;
using Domain.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static Application.Result.Result<Application.DTOs.PropertyDTOS.PropertyDisplayDTO>;
using Domain.Enums.Property;

namespace Application.Services
{
    public class PropertyService
    {
        private readonly IFileService fileService;

        public PropertyService(
                IUnitOfWork _unitOfWork,
                IMapper mapper,
                IFileService _fileService
            )
        {
            UnitOfWork = _unitOfWork;
            Mapper = mapper;
            fileService = _fileService;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        //private User UserExists(string id)
        //{
        //    UnitOfWork.
        //}

        public Result<List<PropertyImageDisplayDTO>> GetImagesByPropertyId(int propertyId)
        {
            var images = UnitOfWork.PropertyImageRepo.GetImagesByPropertyId(propertyId);

            var imageDTOs = images.Select(img => new PropertyImageDisplayDTO
            {
                Id = img.Id,
                GroupName = img.GroupName,
                PropertyId = img.PropertyId,
                ImageUrl = img.ImageUrl,
                IsCover = img.IsCover,
                IsDeleted = img.IsDeleted
            }).ToList();

            return Result<List<PropertyImageDisplayDTO>>.Success(imageDTOs);
        }


        public Result<List<PropertyDisplayDTO>> GetAll()
        {
            var props = UnitOfWork.PropertyRepo.GetAll();
            if (props == null)
                return Result<List<PropertyDisplayDTO>>.Fail(
                    "No properties found",
                    (int)HttpStatusCode.NotFound
                );

            var mapped = Mapper.Map<List<PropertyDisplayDTO>>(props);

            return Result<List<PropertyDisplayDTO>>.Success(mapped);
        }
        public async Task<Result<List<PropertyDisplayWithHostDataDto>>> GetAllForDashboardAsync()
        {
            var props = await UnitOfWork.PropertyRepo.GetAllWithHostDataAsync();
            if (props == null)
                return Result<List<PropertyDisplayWithHostDataDto>>.Fail(
                    "No properties found",
                    (int)HttpStatusCode.NotFound
                );

            var mapped = Mapper.Map<List<PropertyDisplayWithHostDataDto>>(props);

            return Result<List<PropertyDisplayWithHostDataDto>>.Success(mapped);
        }

        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetPageAsync(int page=1, int pageSize = 7, string userId=null)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetPageWithCoverAsync(page, pageSize,userId);
            foreach (var item in paginatedResult.Items)
            {
                Console.WriteLine($"from getPageWithCover {item.WishlistProperties.Count}");
            }
            var mapped = Mapper.Map<PaginatedResult<PropertyDisplayDTO>>(paginatedResult);

            return Result<PaginatedResult<PropertyDisplayDTO>>.Success(mapped);
        }



        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetNearestPageAsync(IpLocation ipLocation ,int page=1, int pageSize = 7, double maxDistanceKm = 10)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetNearestPageWithCoverAsync(ipLocation,page, pageSize,maxDistanceKm);

            var mapped = Mapper.Map<PaginatedResult<PropertyDisplayDTO>>(paginatedResult);

            return Result<PaginatedResult<PropertyDisplayDTO>>.Success(mapped);
        }


        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetFilteredPageAsync(PropertyFilterDto filterDto,string userId)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetFilteredPageAsync(filterDto,userId);

            var mapped = Mapper.Map<PaginatedResult<PropertyDisplayDTO>>(paginatedResult);

            return Result<PaginatedResult<PropertyDisplayDTO>>.Success(mapped);
        }


        public Result<PropertyDisplayDTO> Get(int id)
        {
            var property = UnitOfWork.PropertyRepo.GetById(id);

            if (property == null)
                return Fail("Property not found!", (int)HttpStatusCode.NotFound);

            var mapped = Mapper.Map<PropertyDisplayDTO>(property);

            return Success(mapped);
        }
        public async Task<Result<PropertyDisplayWithHostDataDto>> GetByIdWithCoverAsync(int id)
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdWithCoverAsync(id);

            if (property == null)
                return Result<PropertyDisplayWithHostDataDto>.Fail("Property not found!", (int)HttpStatusCode.NotFound);

            var mapped = Mapper.Map<PropertyDisplayWithHostDataDto>(property);

            return Result<PropertyDisplayWithHostDataDto>.Success(mapped);
        }



        public async Task<Result<List<PropertyDisplayDTO>>> GetByHostIdAsync(string hostId)
        {
            var host = UnitOfWork.UserRepo.GetById(hostId);
            var properties = await UnitOfWork.PropertyRepo.GetByHostIdAsync(hostId);
            var mapped = Mapper.Map<List<PropertyDisplayDTO>>(properties);
            return Result<List<PropertyDisplayDTO>>.Success(mapped);
        }
        public async Task<Result<List<PropertyDisplayWithHostDataDto>>> GetByHostIdWithCoverAsync(string hostId)
        {
            var host = UnitOfWork.UserRepo.GetById(hostId);
            var properties = await UnitOfWork.PropertyRepo.GetByHostIdWithCoverAsync(hostId);
            var mapped = Mapper.Map<List<PropertyDisplayWithHostDataDto>>(properties);
            return Result<List<PropertyDisplayWithHostDataDto>>.Success(mapped);
        }
        public async Task<Result<List<PropertyDisplayWithHostDataDto>>> GetHostListingsWithCoverAsync(string hostId)
        {
            var host = UnitOfWork
                            .UserRepo
                            .GetById(hostId);

            var properties = await UnitOfWork
                                        .PropertyRepo
                                        .GetHostListingsWithCoverAsync(hostId);

            var mapped = Mapper
                            .Map<List<PropertyDisplayWithHostDataDto>>(properties);

            return Result<List<PropertyDisplayWithHostDataDto>>.Success(mapped);
        }

        public Result<PropertyDisplayDTO> Add(PropertyDisplayDTO propertyDTO)
        {
            if (string.IsNullOrEmpty(propertyDTO.HostId.Trim()))
                return Fail("Couldn't add new property", (int)HttpStatusCode.BadRequest);

            var prop = Mapper.Map<Property>(propertyDTO);

            try
            {
                UnitOfWork.PropertyRepo.Add(prop);
                var success = UnitOfWork.SaveChanges() > 0;

                if (!success)
                    return Fail("Couldn't add a new property", (int)HttpStatusCode.BadRequest);

                UnitOfWork.SaveChanges();

                Mapper.Map(prop,propertyDTO);
                return Success(propertyDTO);
            }
            catch
            {
                return Fail("Reference confliction", (int)HttpStatusCode.Conflict);
            }
        }

        public Result<PropertyDisplayDTO> Update(PropertyDisplayDTO propertyDTO)
        {
            var prop = UnitOfWork.PropertyRepo.GetById(propertyDTO.Id);

            if (prop == null)
                return Fail("Property doesn't exist", (int)HttpStatusCode.NotFound);

            if (propertyDTO.HostId != prop.HostId)
                return Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);

            Mapper.Map(propertyDTO, prop);

            try
            {
                UnitOfWork.PropertyRepo.Update(prop);
                var success = UnitOfWork.SaveChanges() > 0;

                if (!success)
                    return Fail("Couldn't update this Property", (int)HttpStatusCode.BadRequest);
                return Success(propertyDTO);
            }
            catch
            {
                return Fail("Reference confliction", (int)HttpStatusCode.Conflict);
            }
        }

        public async Task<Result<bool>> Accept(int propertyId)
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdAsync(propertyId);
            if (property == null)
                return Result<bool>.Fail("not found",(int)HttpStatusCode.NotFound);

            if (property.Status == PropertyAcceptStatus.Rejected)
                return Result<bool>.Fail("Property has already been rejected",(int)HttpStatusCode.NotFound);

                property.Status = PropertyAcceptStatus.Accepted;
            UnitOfWork.PropertyRepo.Update(property);
            await UnitOfWork.SaveChangesAsync();
            
            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> Reject(int propertyId)
        {

            var property = await UnitOfWork.PropertyRepo.GetByIdAsync(propertyId);

            if (property == null)
                return Result<bool>.Fail("not found",(int)HttpStatusCode.NotFound);
            if(property.Status == PropertyAcceptStatus.Accepted)
                return Result<bool>.Fail("Property has already been accepted",(int)HttpStatusCode.NotFound);

            property.Status = PropertyAcceptStatus.Rejected;
            UnitOfWork.PropertyRepo.Update(property);
            await UnitOfWork.SaveChangesAsync();
            
            return Result<bool>.Success(true);
        }

        public Result<PropertyDisplayDTO> Delete(int id, string hostId)
        {
            var property = UnitOfWork.PropertyRepo.GetById(id);
            if (property == null)
                return Fail("Property not found!", (int)HttpStatusCode.NotFound);
            if (property.HostId != hostId)
                return Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);
            try
            {
                property.IsDeleted = true;
                UnitOfWork.PropertyRepo.Update(property);
                var success = UnitOfWork.SaveChanges() > 0;

                if (!success)
                    Fail("Couldn't delete this property", (int)HttpStatusCode.BadRequest);

                var mapped = Mapper.Map<PropertyDisplayDTO>(property);

                return Success(mapped);
            }
            catch
            {
                return Fail("Couldn't delete this property", (int)HttpStatusCode.Conflict);
            }
        }

        //public Result<PropertyImageDisplayDTO> AddImage(PropertyImageCreateDTO propImageCreateDTO)
        //{
        //    var property = UnitOfWork.PropertyRepo.GetById(propImageCreateDTO.PropertyId);
        //    if (property == null)
        //        return Result.Result<PropertyImageDisplayDTO>.Fail("Property not found",(int)HttpStatusCode.NotFound);

        //    if (property.HostId != propImageCreateDTO.HostId)
        //            Result<PropertyImageDisplayDTO>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);

        //    var propertyImage = Mapper.Map<PropertyImage>(propImageCreateDTO);
        //    try
        //    {
        //        UnitOfWork.PropertyImageRepo.Add(propertyImage);
        //        bool succed = UnitOfWork.SaveChanges() > 0;

        //        if (succed)
        //        {
        //            var propDisDto = Mapper.Map<PropertyImageDisplayDTO>(propertyImage);
        //            return Result<PropertyImageDisplayDTO>.Success(propDisDto, (int)HttpStatusCode.Created, "Image uploaded");
        //        }

        //        return Result<PropertyImageDisplayDTO>.Fail("Faild to uploaded", (int)HttpStatusCode.BadRequest);
        //    }
        //    catch
        //    {
        //        return Result<PropertyImageDisplayDTO>.Fail("Couldn't upload the image",(int)HttpStatusCode.InternalServerError);
        //    }
        //}
        public Result<List<PropertyImageDisplayDTO>> AddImages(PropertyImagesCreateContainerDTO dto)
        {
            var property = UnitOfWork.PropertyRepo.GetById(dto.PropertyId);
            if (property == null)
                return Result<List<PropertyImageDisplayDTO>>.Fail(
                    "Property not found",
                    (int)HttpStatusCode.NotFound
                );

            if (property.HostId != dto.HostId)
                return Result<List<PropertyImageDisplayDTO>>.Fail(
                    "Unauthorized",
                    (int)HttpStatusCode.Unauthorized
                );

            var imageEntities = new List<PropertyImage>();

            foreach (var imageDto in dto.Images)
            {
                var image = new PropertyImage
                {
                    GroupName = imageDto.GroupName,
                    PropertyId = dto.PropertyId,
                    ImageUrl = imageDto.ImageUrl,
                    IsCover = imageDto.IsCover,
                };

                imageEntities.Add(image);
                UnitOfWork.PropertyImageRepo.Add(image);
            }

            try
            {
                bool saved = UnitOfWork.SaveChanges() > 0;

                if (saved)
                {
                    var result = imageEntities
                        .Select(img => Mapper.Map<PropertyImageDisplayDTO>(img))
                        .ToList();

                    return Result<List<PropertyImageDisplayDTO>>.Success(
                        result,
                        (int)HttpStatusCode.Created,
                        "Images uploaded"
                    );
                }

                return Result<List<PropertyImageDisplayDTO>>.Fail(
                    "Images not uploaded",
                    (int)HttpStatusCode.BadRequest
                );
            }
            catch (Exception e)
            {
                return Result<List<PropertyImageDisplayDTO>>.Fail(
                    "Couldn't upload the images " + e.Message,
                    (int)HttpStatusCode.InternalServerError
                );
            }
        }



        public async Task<Result<bool>> DeleteImages(int[] imgIds, int propertyId, string userId, string rootPath, string webPath )
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdAsync( propertyId );
            if(property == null )
                return Result<bool>.Fail("not found",(int)HttpStatusCode.NotFound);
            if(property.HostId != userId)
                return Result<bool>.Fail("not allowed", (int)HttpStatusCode.Unauthorized);

            var imgs = await UnitOfWork.PropertyImageRepo.GetRangeAsync(imgIds, propertyId);

            foreach(var img in imgs)
            {
                await fileService.MoveAsync(webPath + img.ImageUrl, Path.Combine(rootPath, "private", "uploads"));
                img.ImageUrl = Path.GetFileName(img.ImageUrl);
            }
            await UnitOfWork.PropertyImageRepo.DeleteRangeAsync(imgs);
            await UnitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true, 204);
        }

        public async Task<Result<string>> Deactivate(int propertyId)
        {
            var property = UnitOfWork.PropertyRepo.GetById( propertyId );

            if (property == null)
                return Result<string>.Fail("Property not found", (int) HttpStatusCode.NotFound);

            if (!property.IsActive)
                return Result<string>.Success("Property alrady inactive ", (int) HttpStatusCode.NoContent);

            property.IsActive = false;

            UnitOfWork.PropertyRepo.Update(property);

            await UnitOfWork.SaveChangesAsync();

            return Result<string>.Success("Proeprty deactivated",(int)HttpStatusCode.NoContent);

        }
        public async Task<Result<string>> Activate(int propertyId)
        {
            var property = UnitOfWork.PropertyRepo.GetById( propertyId );

            if (property == null)
                return Result<string>.Fail("Property not found", (int) HttpStatusCode.NotFound);

            if(property.IsActive)
                return Result<string>.Success("Property already active", (int) HttpStatusCode.NoContent);

            property.IsActive = true;

            UnitOfWork.PropertyRepo.Update(property);

            await UnitOfWork.SaveChangesAsync();

            return Result<string>.Success("Proeprty deactivated",(int)HttpStatusCode.NoContent);

        }

    }
}
