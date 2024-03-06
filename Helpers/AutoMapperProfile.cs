using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class AutoMapperProfile : Profile
    {
        protected AutoMapperProfile()
        {
            CreateMap<AccountModel, AccountSimpleDTO>();
            CreateMap<AccountSimpleDTO, AccountModel>();
            CreateMap<AccountModel, AccountEagerDTO>();
            CreateMap<AccountEagerDTO, AccountModel>();
            CreateMap<AccountSimpleDTO, AccountEagerDTO>();
            CreateMap<AccountEagerDTO, AccountSimpleDTO>();
        }
    }
}