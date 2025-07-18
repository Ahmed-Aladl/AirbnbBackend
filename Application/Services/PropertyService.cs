using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.Interfaces;

using Application.Result;
using AutoMapper;
using AutoMapper.Features;
using AutoMapper.Internal;
using Domain.Models;
using static Application.Result.Result<Application.DTOs.PropertyDTOS.PropertyDisplayDTO>;

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

        public Result<List<PropertyDisplayDTO>> GetAll()
        {

            var props = UnitOfWork
                            .PropertyRepo
                            .GetAll();
            if (props == null)
                return Result<List<PropertyDisplayDTO>>.Fail("No properties found",(int)HttpStatusCode.NotFound);


            var mapped = Mapper
                            .Map<List<PropertyDisplayDTO>>(props);
                    
            return Result<List<PropertyDisplayDTO>>
                                                .Success(mapped);

                
        }

        public Result<PropertyDisplayDTO> Get(int id) 
        {
            var config = Mapper.ConfigurationProvider.Internal() ;
            foreach (var map in config.GetAllTypeMaps())
            {
                Console.WriteLine($"Mapped: {map.SourceType.FullName} => {map.DestinationType.FullName}");
            }
            var property = UnitOfWork
                            .PropertyRepo
                            .GetById(id);

            if(property == null)
                    Fail("Property not found!",(int)HttpStatusCode.NotFound);


            var mapped = Mapper
                            .Map<PropertyDisplayDTO>(property);

            return Success(mapped);
        }

        public Result<PropertyDisplayDTO> Add(PropertyDisplayDTO propertyDTO)
        {
            if (propertyDTO.Id == 0)
                return Fail("Couldn't add new property", (int)HttpStatusCode.BadRequest);
            

            var prop = Mapper.Map<Property>(propertyDTO);

            try
            {

                UnitOfWork.PropertyRepo.Add(prop);
                var success = UnitOfWork.SaveChanges() > 0;

                if (!success)
                    return Fail("Couldn't add a new property", (int)HttpStatusCode.BadRequest);
                return Success(propertyDTO);
            }
            catch
            {
                return Fail("Reference confliction", (int) HttpStatusCode.Conflict);
            }
        }
        public Result<PropertyDisplayDTO> Update(PropertyDisplayDTO propertyDTO)
        {
            var prop =  UnitOfWork.PropertyRepo
                                  .GetById(propertyDTO.Id);

            if (prop == null)
                return Fail("Property doesn't exist", (int) HttpStatusCode.NotFound);

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
                return Fail("Reference confliction", (int) HttpStatusCode.Conflict);
            }
        }


        public Result<PropertyDisplayDTO> Delete(int id) 
        {
            var property = UnitOfWork
                            .PropertyRepo
                            .GetById(id);
            if(property == null)
                   return Fail("Property not found!", (int)HttpStatusCode.NotFound);


            try
            {
                property.IsDeleted = true;
                UnitOfWork.PropertyRepo.Update(property);
                var success = UnitOfWork.SaveChanges()> 0;

                if (!success)
                    Fail("Couldn't delete this property", (int)HttpStatusCode.BadRequest);

                var mapped= Mapper
                                 .Map<PropertyDisplayDTO>(property);

                return Success(mapped);
            }
            catch
            {
                return Fail("Couldn't delete this property", (int)HttpStatusCode.Conflict);
            }
        }



    }
}
