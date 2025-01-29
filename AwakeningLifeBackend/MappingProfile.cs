using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserForRegistrationDto, User>();
        CreateMap<User, UserDto>();

        CreateMap<BaseEntity, BaseEntityDto>()
            .ForMember(c => c.FullAddress,
                opt => opt.MapFrom(x => $"{x.Address} {x.Country}"));
        CreateMap<BaseEntityForCreationDto, BaseEntity>();
        CreateMap<BaseEntityForUpdateDto, BaseEntity>();

        CreateMap<DependantEntity, DependantEntityDto>();
        CreateMap<DependantEntityForCreationDto, DependantEntity>();
        CreateMap<DependantEntityForUpdateDto, DependantEntity>().ReverseMap();
    }
}
