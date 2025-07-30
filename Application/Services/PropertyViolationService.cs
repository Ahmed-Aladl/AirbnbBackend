using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyViolationDTOs;
using Application.Interfaces;
using Application.Result;
using Domain.Enums.PropertyViolations;
using Domain.Models;

namespace Application.Services
{
    public class PropertyViolationService
    {
        private readonly IUnitOfWork _uow;

        public PropertyViolationService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<Result<List<PropertyViolationDetailsDTO>>> GetAllPropertyViolationsAsync()
        {
            try
            {
                var violations = await _uow.PropertyViolationRepo.GetAllAsync();
                var violationDtos = MapToDetailsDTOs(violations);

                return Result<List<PropertyViolationDetailsDTO>>.Success(violationDtos);
            }
            catch (Exception)
            {
                return Result<List<PropertyViolationDetailsDTO>>.Fail("Failed to retrieve property violations.", 500);
            }
        }

        public async Task<Result<PropertyViolationDetailsDTO>> GetPropertyViolationByIdAsync(int id)
        {
            try
            {
                var violation = await _uow.PropertyViolationRepo.GetByIdAsync(id);

                if (violation == null)
                    return Result<PropertyViolationDetailsDTO>.Fail("Violation not found", 404);

                var dto = MapToDetailsDTO(violation);
                return Result<PropertyViolationDetailsDTO>.Success(dto);
            }
            catch (Exception)
            {
                return Result<PropertyViolationDetailsDTO>.Fail("Failed to retrieve violation.", 500);
            }
        }

        public async Task<Result<List<PropertyViolationDetailsDTO>>> GetViolationsByUserIdAsync(string userId)
        {
            try
            {
                var user = _uow.UserRepo.GetById(userId);
                if (user == null)
                    return Result<List<PropertyViolationDetailsDTO>>.Fail("User not found.", 404);

                var violations = await _uow.PropertyViolationRepo.GetViolationsByUserIdAsync(userId);

                if (violations == null || !violations.Any())
                {
                    return Result<List<PropertyViolationDetailsDTO>>.Success(new List<PropertyViolationDetailsDTO>());
                }

                var violationDtos = MapToDetailsDTOs(violations);
                return Result<List<PropertyViolationDetailsDTO>>.Success(violationDtos);
            }
            catch (Exception)
            {
                return Result<List<PropertyViolationDetailsDTO>>.Fail("Failed to retrieve violations for this user.", 500);
            }
        }

        public async Task<Result<List<PropertyViolationDetailsDTO>>> GetViolationsByPropertyIdAsync(int propertyId)
        {
            try
            {
                var property = await _uow.PropertyRepo.GetByIdAsync(propertyId);
                if (property == null)
                    return Result<List<PropertyViolationDetailsDTO>>.Fail("Property not found.", 404);

                var violations = await _uow.PropertyViolationRepo.GetViolationsByPropertyIdAsync(propertyId);

                if (violations == null || !violations.Any())
                {
                    return Result<List<PropertyViolationDetailsDTO>>.Success(new List<PropertyViolationDetailsDTO>());
                }

                var violationDtos = MapToDetailsDTOs(violations);
                return Result<List<PropertyViolationDetailsDTO>>.Success(violationDtos);
            }
            catch (Exception)
            {
                return Result<List<PropertyViolationDetailsDTO>>.Fail("Failed to retrieve violations for this property.", 500);
            }
        }

        public async Task<Result<string>> UpdateViolationAsync(UpdateViolationDTO dto)
        {
            try
            {
                var violation = await _uow.PropertyViolationRepo.GetByIdAsync(dto.Id);

                if (violation == null)
                    return Result<string>.Fail("Violation not found.", 404);

                if (!Enum.TryParse(dto.Status, out PropertyViolationsStatus newStatus))
                    return Result<string>.Fail("Invalid status value.", 400);

                violation.Status = newStatus;
                violation.AdminNotes = dto.AdminNotes;

                _uow.PropertyViolationRepo.Update(violation);
                await _uow.SaveChangesAsync();

                return Result<string>.Success("Violation updated successfully.");
            }
            catch (Exception)
            {
                return Result<string>.Fail("Failed to update violation.", 500);
            }
        }

        public async Task<Result<string>> AddViolationAsync(CreateViolationDTO dto)
        {
            try
            {
                var property = await _uow.PropertyRepo.GetByIdAsync(dto.PropertyId);
                if (property == null)
                    return Result<string>.Fail("Property not found.", 404);

                var user = _uow.UserRepo.GetById(dto.UserId);
                if (user == null)
                    return Result<string>.Fail("User not found.", 404);

                var existingViolations = await _uow.PropertyViolationRepo.GetViolationsByUserIdAsync(dto.UserId);
                bool hasDuplicatePending = existingViolations.Any(v =>
                    v.PropertyId == dto.PropertyId &&
                    v.Status == PropertyViolationsStatus.Pending);

                if (hasDuplicatePending)
                    return Result<string>.Fail("You have already submitted a pending violation for this property.", 400);

                var violation = new PropertyViolation
                {
                    Reason = dto.Reason,
                    UserId = dto.UserId,
                    PropertyId = dto.PropertyId,
                    CreatedAt = DateTime.UtcNow,
                    Status = PropertyViolationsStatus.Pending,
                    AdminNotes = null
                };

                await _uow.PropertyViolationRepo.AddAsync(violation);
                await _uow.SaveChangesAsync();

                return Result<string>.Success("Violation submitted successfully. Pending admin review.");
            }
            catch (Exception)
            {
                return Result<string>.Fail("Failed to submit property violation.", 500);
            }
        }

        private List<PropertyViolationDetailsDTO> MapToDetailsDTOs(List<PropertyViolation> violations)
        {
            return violations.Select(MapToDetailsDTO).ToList();
        }

        private PropertyViolationDetailsDTO MapToDetailsDTO(PropertyViolation violation)
        {
            return new PropertyViolationDetailsDTO
            {
                Id = violation.Id,
                Reason = violation.Reason,
                CreatedAt = violation.CreatedAt,
                Status = violation.Status.ToString(),
                AdminNotes = violation.AdminNotes,
                UserId = violation.UserId,
                FirstName = violation.User?.FirstName,
                LastName = violation.User?.LastName,
                UserEmail = violation.User?.Email,
                PropertyId = violation.PropertyId,
                PropertyTitle = violation.Property?.Title ?? string.Empty,
                City = violation.Property?.City ?? string.Empty,
                Country = violation.Property?.Country ?? string.Empty
            };
        }
    }
}