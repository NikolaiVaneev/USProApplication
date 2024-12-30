using AutoMapper;

namespace USProApplication.DataBase.Mappings;

public class ServiceMap : Profile
{
    public ServiceMap()
    {
        CreateMap<Entities.Service, Models.Service>();
        CreateMap<Models.Service, Entities.Service>();
    }
}
