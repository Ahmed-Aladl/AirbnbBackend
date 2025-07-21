using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyImageDTOs;
using Application.Interfaces;
using Application.Result;
using Application.Shared;
using AutoMapper;
using AutoMapper.Features;
using AutoMapper.Internal;
using Domain.Models;
using static Application.Result.Result<Application.DTOs.PropertyDTOS.PropertyDisplayDTO>;

namespace Application.Services
{
    public class PropertyService
    {
        public PropertyService(IUnitOfWork _unitOfWork, IMapper mapper)
        {
            UnitOfWork = _unitOfWork;
            Mapper = mapper;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        //private User UserExists(string id)
        //{
        //    UnitOfWork.
        //}

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

        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetPageAsync(int page=1, int pageSize = 7)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetPageWithCoverAsync(page, pageSize);

            var mapped = Mapper.Map<PaginatedResult<PropertyDisplayDTO>>(paginatedResult);

            return Result<PaginatedResult<PropertyDisplayDTO>>.Success(mapped);
        }


        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetNearestPageAsync(IpLocation ipLocation ,int page=1, int pageSize = 7, double maxDistanceKm = 10)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetNearestPageWithCoverAsync(ipLocation,page, pageSize,maxDistanceKm);

            var mapped = Mapper.Map<PaginatedResult<PropertyDisplayDTO>>(paginatedResult);

            return Result<PaginatedResult<PropertyDisplayDTO>>.Success(mapped);
        }


        public async Task<Result<PaginatedResult<PropertyDisplayDTO>>> GetFilteredPageAsync(PropertyFilterDto filterDto)
        {
            var paginatedResult = await UnitOfWork.PropertyRepo.GetFilteredPageAsync(filterDto);

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
        public async Task<Result<PropertyDisplayDTO>> GetByIdWithCoverAsync(int id)
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdWithCoverAsync(id);

            if (property == null)
                return Fail("Property not found!", (int)HttpStatusCode.NotFound);

            var mapped = Mapper.Map<PropertyDisplayDTO>(property);

            return Success(mapped);
        }



        public async Task<Result<List<PropertyDisplayDTO>>> GetByHostIdAsync(string hostId)
        {
            var host = UnitOfWork.UserRepo.GetById(hostId);
            var properties = await UnitOfWork.PropertyRepo.GetByHostIdAsync(hostId);
            var mapped = Mapper.Map<List<PropertyDisplayDTO>>(properties);
            return Result<List<PropertyDisplayDTO>>.Success(mapped);
        }
        public async Task<Result<List<PropertyDisplayDTO>>> GetByHostIdWithCoverAsync(string hostId)
        {
            var host = UnitOfWork.UserRepo.GetById(hostId);
            var properties = await UnitOfWork.PropertyRepo.GetByHostIdWithCoverAsync(hostId);
            var mapped = Mapper.Map<List<PropertyDisplayDTO>>(properties);
            return Result<List<PropertyDisplayDTO>>.Success(mapped);
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

        public Result<PropertyDisplayDTO> Delete(int id)
        {
            var property = UnitOfWork.PropertyRepo.GetById(id);
            if (property == null)
                return Fail("Property not found!", (int)HttpStatusCode.NotFound);

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
    }
}
