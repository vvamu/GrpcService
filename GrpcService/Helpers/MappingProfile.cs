using AutoMapper;

namespace GrpcService.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CustomerRequest, CustomerDTORequest>().ReverseMap();
        CreateMap<CustomerDTORequest, CustomerResponse>();

    }
}