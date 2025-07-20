using System.Net;
using Application.DTOs.AmenityDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;
using static Application.Result.Result<Application.DTOs.AmenityDTOs.AmenityDTO>;

namespace Application.Services;

public class AmenityService
{
    public IUnitOfWork _unitOfWork;
    public IMapper _mapper;

    public AmenityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    //public async Task<Result<AmenityDTO>> CreateAsync(CreateAmenityDTO createAmenityDto)
    //{
    //    if (string.IsNullOrWhiteSpace(createAmenityDto.AmenityName))
    //        return Fail("Couldn't add new amenity", (int)HttpStatusCode.BadRequest);

    //    var amenity = _mapper.Map<Amenity>(createAmenityDto);

    //    try
    //    {
    //        _unitOfWork.AmenitiesRepo.Add(amenity);
    //        var success = _unitOfWork.SaveChanges() > 0;

    //        if (!success)
    //            return Fail("Couldn't add a new amenity", (int)HttpStatusCode.BadRequest);

    //        var resultDto = _mapper.Map<AmenityDTO>(amenity);
    //        return Success(resultDto);
    //    }
    //    catch
    //    {
    //        return Fail("Reference confliction", (int)HttpStatusCode.Conflict);
    //    }
    //}

    public async Task<Result<AmenityDTO>> CreateAsync(CreateAmenityDTO dto)
    {
        if (dto.IconUrl == null || dto.IconUrl.Length == 0)
            return Result<AmenityDTO>.Fail("Icon file is required", 400);

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.IconUrl.FileName);
        string filePath = Path.Combine("wwwroot", "AmenitiesIcons", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.IconUrl.CopyToAsync(stream);
        }

        string iconUrl = $"/uploads/amenities/{fileName}";

        var amenity = new Amenity { AmenityName = dto.AmenityName, IconURL = iconUrl };

        _unitOfWork.AmenitiesRepo.Add(amenity);
        var success = _unitOfWork.SaveChanges() > 0;

        if (!success)
            return Result<AmenityDTO>.Fail("Failed to save amenity", 500);

        var resultDto = _mapper.Map<AmenityDTO>(amenity);
        return Result<AmenityDTO>.Success(resultDto);
    }

    public async Task<Result<AmenityDTO>> GetAmenityById(int amenityId)
    {
        try
        {
            var amenity = await _unitOfWork.AmenitiesRepo.GetAmenityByIdAsync(amenityId);

            var result = _mapper.Map<AmenityDTO>(amenity);
            return Result<AmenityDTO>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<AmenityDTO>.Fail($"Error retrieving amentity: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<AmenityDTO>>> GetAllAmenitiesByPropertyIdAsync(
        int proprtyId
    )
    {
        try
        {
            var amenities = await _unitOfWork.AmenitiesRepo.GetAmenitiesByPropertyIdAsync(
                proprtyId
            );
            var result = _mapper.Map<IEnumerable<AmenityDTO>>(amenities);
            return Result<IEnumerable<AmenityDTO>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<AmenityDTO>>.Fail(
                $"Error retrieving amenities: {ex.Message}",
                500
            );
        }
    }

    public async Task<Result<IEnumerable<AmenityDTO>>> GetAllAmenitiesAsync()
    {
        try
        {
            var amenities = await _unitOfWork.AmenitiesRepo.GetAllAmenitiesAsync();
            var result = _mapper.Map<IEnumerable<AmenityDTO>>(amenities);
            return Result<IEnumerable<AmenityDTO>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<AmenityDTO>>.Fail(
                $"Error retrieving amenities: {ex.Message}",
                500
            );
        }
    }

    public async Task<Result<AmenityDTO>> Update(AmenityDTO amenityUpdateDTO)
    {
        var amenity = await _unitOfWork.AmenitiesRepo.GetAmenityByIdAsync(amenityUpdateDTO.ID);

        if (amenity == null)
            return Fail("This Amenity doesn't exist", (int)HttpStatusCode.NotFound);

        _mapper.Map(amenityUpdateDTO, amenity);

        try
        {
            _unitOfWork.AmenitiesRepo.Update(amenity);
            var success = _unitOfWork.SaveChanges() > 0;

            if (!success)
                return Fail("Couldn't update this Amenity", (int)HttpStatusCode.BadRequest);

            return Success(amenityUpdateDTO);
        }
        catch
        {
            return Fail("Reference confliction", (int)HttpStatusCode.Conflict);
        }
    }

    public async Task<Result<string>> DeleteAsync(int amenityId)
    {
        var repo = _unitOfWork.AmenitiesRepo;

        var amenity = await repo.GetAmenityByIdAsync(amenityId);
        if (amenity == null)
            return Result<string>.Fail("Amenity not found", 404);

        repo.Delete(amenity);
        var success = await _unitOfWork.SaveChangesAsync() > 0;

        if (!success)
            return Result<string>.Fail("Failed to delete amenity", 400);

        return Result<string>.Success("Amenity deleted successfully");
    }
}
