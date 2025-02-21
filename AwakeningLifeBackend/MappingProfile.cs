using AutoMapper;
using AwakeningLifeBackend.Core.Domain.Entities;
using Shared.DataTransferObjects;
using Stripe;

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

        CreateMap<Price, SubServicePriceDto>()
            .ForMember(p => p.EstimatedRenewalDate, opt => opt.MapFrom(x => GetEstimatedRenewalDate(x)))
            .ForMember(p => p.PriceId, opt => opt.MapFrom(x => x.Id));
        CreateMap<Product, SubServiceProductDto>()
            .ForMember(p => p.ProductId, opt => opt.MapFrom(x => x.Id));
        CreateMap<Customer, SubServiceCustomerDto>()
            .ForMember(c => c.CustomerId, opt => opt.MapFrom(x => x.Id));
        CreateMap<Subscription, SubServiceSubscriptionDto>()
            .ForMember(s => s.SubscriptionId, opt => opt.MapFrom(x => x.Id));
    }

    public static DateTime GetEstimatedRenewalDate(Price price)
    {
        return price.Recurring.Interval switch
        {
            "day" => DateTime.Now.AddDays(1),
            "week" => DateTime.Now.AddDays(7),
            "month" => DateTime.Now.AddMonths(1),
            "year" => DateTime.Now.AddYears(1),
            _ => DateTime.Now
        };
    }
}
