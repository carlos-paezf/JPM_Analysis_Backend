using AutoMapper;
using BackendJPMAnalysis.DTO;
using BackendJPMAnalysis.Models;


namespace BackendJPMAnalysis.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AccountModel, AccountSimpleDTO>();
            CreateMap<AccountSimpleDTO, AccountModel>();
            CreateMap<AccountModel, AccountEagerDTO>();
            CreateMap<AccountEagerDTO, AccountModel>();
            CreateMap<AccountSimpleDTO, AccountEagerDTO>();
            CreateMap<AccountEagerDTO, AccountSimpleDTO>();

            CreateMap<ClientModel, ClientSimpleDTO>();
            CreateMap<ClientSimpleDTO, ClientModel>();
            CreateMap<ClientModel, ClientEagerDTO>();
            CreateMap<ClientEagerDTO, ClientModel>();
            CreateMap<ClientSimpleDTO, ClientEagerDTO>();
            CreateMap<ClientEagerDTO, ClientSimpleDTO>();

            CreateMap<CompanyUserModel, CompanyUserSimpleDTO>();
            CreateMap<CompanyUserSimpleDTO, CompanyUserModel>();
            CreateMap<CompanyUserModel, CompanyUserEagerDTO>();
            CreateMap<CompanyUserEagerDTO, CompanyUserModel>();
            CreateMap<CompanyUserSimpleDTO, CompanyUserEagerDTO>();
            CreateMap<CompanyUserEagerDTO, CompanyUserSimpleDTO>();

            CreateMap<FunctionModel, FunctionSimpleDTO>();
            CreateMap<FunctionSimpleDTO, FunctionModel>();
            CreateMap<FunctionModel, FunctionEagerDTO>();
            CreateMap<FunctionEagerDTO, FunctionModel>();
            CreateMap<FunctionSimpleDTO, FunctionEagerDTO>();
            CreateMap<FunctionEagerDTO, FunctionSimpleDTO>();

            CreateMap<ProductModel, ProductSimpleDTO>();
            CreateMap<ProductSimpleDTO, ProductModel>();
            CreateMap<ProductModel, ProductEagerDTO>();
            CreateMap<ProductEagerDTO, ProductModel>();
            CreateMap<ProductSimpleDTO, ProductEagerDTO>();
            CreateMap<ProductEagerDTO, ProductSimpleDTO>();

            CreateMap<ProfileModel, ProfileSimpleDTO>();
            CreateMap<ProfileSimpleDTO, ProfileModel>();
            CreateMap<ProfileModel, ProfileEagerDTO>();
            CreateMap<ProfileEagerDTO, ProfileModel>();
            CreateMap<ProfileSimpleDTO, ProfileEagerDTO>();
            CreateMap<ProfileEagerDTO, ProfileSimpleDTO>();
        }
    }
}