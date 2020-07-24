using AutoMapper;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(cdto => cdto.FullAddress, opt => 
                {
                    opt.MapFrom(c => string.Join(" ", c.Address, c.Country));
                });

            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeForCreationDto, Employee>();


            CreateMap<Company, CompanyForCreationDto>();
            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
            CreateMap<CompanyForUpdateDto, Company>();
        }
    }
}
