using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Models;
using static Application.Responses.ApiResponse<Application.DTOs.PropertyDTOS.PropertyDisplayDTO>;

namespace Application.Services
{
    public class PropertyService
    {
        public PropertyService(IUnitOfWork _unitOfWork,IMapper mapper) {
            UnitOfWork = _unitOfWork;
            Mapper = mapper;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        public ApiResponse<List<PropertyDisplayDTO>> GetAll()
        {

            var props = UnitOfWork
                            .PropertyRepo
                            .GetAll();
            if (props == null)
                return ApiResponse<List<PropertyDisplayDTO>>.Fail("No properties found");


            var mapped = Mapper
                            .Map<List<PropertyDisplayDTO>>(props);
                    
            return ApiResponse<List<PropertyDisplayDTO>>
                                                .Success(mapped);

                
        }

        public ApiResponse<PropertyDisplayDTO> Get(int id) 
        {
            var property = UnitOfWork
                            .PropertyRepo
                            .GetById(id);

            if(property == null)
                    Fail("Property not found!");


            var mapped = Mapper
                            .Map<PropertyDisplayDTO>(property);

            return Success(mapped);
        }

        public ApiResponse<PropertyDisplayDTO> Update(PropertyDisplayDTO propertyDTO)
        {
            var prop =  UnitOfWork.PropertyRepo
                                  .GetById(propertyDTO.Id);

            if (prop == null)
                return Fail("Property doesn't exist");
            Mapper.Map(propertyDTO, prop);

            try
            {

                UnitOfWork.PropertyRepo.Update(prop);
                var success = UnitOfWork.SaveChanges() > 0;

                if (!success)
                    return Fail("Couldn't update this Property");
                return Success(propertyDTO);
            }
            catch
            {
                return Fail("Reference confliction");
            }
        }


        public ApiResponse<PropertyDisplayDTO> Delete(int id) 
        {
            var property = UnitOfWork
                            .PropertyRepo
                            .GetById(id);
            if(property == null)
                   return Fail("Property not found!");


            try
            {
                property.IsDeleted = true;
                UnitOfWork.PropertyRepo.Update(property);
                var success = UnitOfWork.SaveChanges()> 0;

                if (!success)
                    Fail("Couldn't delete this property");

                var mapped= Mapper
                                 .Map<PropertyDisplayDTO>(property);

                return Success(mapped, "Successfuly deleted");
            }
            catch
            {
                return Fail("Couldn't delete this property");
            }
        }



    }
}
