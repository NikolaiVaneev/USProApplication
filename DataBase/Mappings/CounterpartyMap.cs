using AutoMapper;
using USProApplication.Models;

namespace USProApplication.DataBase.Mappings;

public class CounterpartyMap : Profile
{
    public CounterpartyMap()
    {
        CreateMap<Entities.Counterparty, ClientShortInfo>()
            .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
            .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
            .ForMember(e => e.ChiefFullName, opts => opts.MapFrom(src => src.Director))
            .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
            //.ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.))
            .ForMember(e => e.IsExecutor, opts => opts.MapFrom(src => src.Executor))
            ;

        CreateMap<CounterpartyDTO, ClientShortInfo>()
            .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
            .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
            .ForMember(e => e.ChiefFullName, opts => opts.MapFrom(src => src.Director))
            .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
            //.ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.))
            .ForMember(e => e.IsExecutor, opts => opts.MapFrom(src => src.Executor));

        CreateMap<CounterpartyDTO, Entities.Counterparty>();
        CreateMap<Entities.Counterparty, CounterpartyDTO>();
    }
}
