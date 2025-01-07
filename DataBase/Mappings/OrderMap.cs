using AutoMapper;
using USProApplication.Models;

namespace USProApplication.DataBase.Mappings
{
    public class OrderMap : Profile
    {
        public OrderMap()
        {
            CreateMap<Entities.Order, OrderShortInfo>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(e => e.Status, opts => opts.MapFrom(src => src.IsCompleted ? "Выполнен" : CalculateStatus(src.StartDate, src.Term)))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.Square))
                .ForMember(e => e.ContractNo, opts => opts.MapFrom(src => src.Number))
                .ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.StartDate))
                .ForMember(e => e.AccountNo, opts => opts.MapFrom(src => $"{src.PrepaymentBillNumber} от {src.PrepaymentBillDate}"));

            //CreateMap<CounterpartyDTO, ClientShortInfo>()
            //    .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
            //    .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
            //    .ForMember(e => e.ChiefFullName, opts => opts.MapFrom(src => src.Director))
            //    .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
            //    //.ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.))
            //    .ForMember(e => e.IsExecutor, opts => opts.MapFrom(src => src.Executor));

            //CreateMap<CounterpartyDTO, Entities.Counterparty>();
            //CreateMap<Entities.Counterparty, CounterpartyDTO>();
        }

        private static string CalculateStatus(DateOnly? startDate, int? term)
        {
            if (!startDate.HasValue || !term.HasValue)
            {
                return "В работе"; // Нет данных для определения статуса
            }

            var endDate = startDate.Value.AddDays(term.Value);
            var today = DateOnly.FromDateTime(DateTime.Today);

            return endDate < today ? "Просрочен" : "В работе";
        }
    }
}
