using AutoMapper;
using Api.Models;
using Api.Dtos.Employee;
using Api.Dtos.Dependent;
using Api.Dtos.Paycheck;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, GetEmployeeDto>();
        CreateMap<Dependent, GetDependentDto>();
        CreateMap<Paycheck, GetPaycheckDto>();
    }
}