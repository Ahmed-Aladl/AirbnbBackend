using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyType;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Enums.Booking;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Application.Services
{
    public class PropertyTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PropertyTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PropertyTypeDto>> GetAllAsync()
        {
            var list = await _unitOfWork.propertyType.GetAllAsync();
            return _mapper.Map<List<PropertyTypeDto>>(list);
        }

        public async Task<PropertyTypeDto?> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.propertyType.GetByIdAsync(id);
            return _mapper.Map<PropertyTypeDto?>(item);
        }


        public async Task AddAsync(PropertyType entity)
        {
            await _unitOfWork.propertyType.AddAsync(entity);
        }

        public void Update(PropertyType entity)
        {
            _unitOfWork.propertyType.Update(entity);
        }

        public void Delete(PropertyType entity)
        {
            _unitOfWork.propertyType.Delete(entity);
        }


    }
}
