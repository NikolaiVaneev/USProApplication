using AutoMapper;
using USProApplication.DataBase.Entities;
using USProApplication.Models;

namespace USProApplication.DataBase.Mappings
{
    public class OrderMap : Profile
    {
        public OrderMap()
        {
            CreateMap<Order, OrderDTO>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(e => e.Number, opts => opts.MapFrom(src => src.Number))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.Square))
                .ForMember(e => e.CustomerId, opts => opts.MapFrom(src => src.CustomerId))
                .ForMember(e => e.Phone, opts => opts.MapFrom(src => src.Phone))
                .ForMember(e => e.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(e => e.ExecutorId, opts => opts.MapFrom(src => src.ExecutorId))
                .ForMember(e => e.IsCompleted, opts => opts.MapFrom(src => src.IsCompleted))
                .ForMember(e => e.StartDate, opts => opts.MapFrom(src => src.StartDate.HasValue
                    ? src.StartDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
                .ForMember(e => e.СompletionDate, opts => opts.MapFrom(src => src.СompletionDate.HasValue
                    ? src.СompletionDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
                .ForMember(e => e.PrepaymentBillDate, opts => opts.MapFrom(src => src.PrepaymentBillDate.HasValue
                    ? src.PrepaymentBillDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
                .ForMember(e => e.ExecutionBillDate, opts => opts.MapFrom(src => src.ExecutionBillDate.HasValue
                    ? src.ExecutionBillDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))
                .ForMember(e => e.ApprovalBillDate, opts => opts.MapFrom(src => src.ApprovalBillDate.HasValue
                    ? src.ApprovalBillDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null))

                .ForMember(e => e.Price, opts => opts.MapFrom(src => src.Price))
                .ForMember(e => e.PriceToMeter, opts => opts.MapFrom(src => src.PriceToMeter))
                .ForMember(e => e.SelectedServicesIds, opts => opts.MapFrom(src => src.Services.Select(s => s.Id)));

            CreateMap<OrderDTO, Order>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(e => e.Number, opts => opts.MapFrom(src => src.Number))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.Square))
                .ForMember(e => e.CustomerId, opts => opts.MapFrom(src => src.CustomerId))
                .ForMember(e => e.Phone, opts => opts.MapFrom(src => src.Phone))
                .ForMember(e => e.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(e => e.ExecutorId, opts => opts.MapFrom(src => src.ExecutorId))
                .ForMember(e => e.IsCompleted, opts => opts.MapFrom(src => src.IsCompleted))
                .ForMember(e => e.StartDate, opts => opts.MapFrom(src => src.StartDate.HasValue
                    ? DateOnly.FromDateTime(src.StartDate.Value)
                    : (DateOnly?)null))
                .ForMember(e => e.СompletionDate, opts => opts.MapFrom(src => src.СompletionDate.HasValue
                    ? DateOnly.FromDateTime(src.СompletionDate.Value)
                    : (DateOnly?)null))
                .ForMember(e => e.PrepaymentBillDate, opts => opts.MapFrom(src => src.PrepaymentBillDate.HasValue
                    ? DateOnly.FromDateTime(src.PrepaymentBillDate.Value)
                    : (DateOnly?)null))
                .ForMember(e => e.ExecutionBillDate, opts => opts.MapFrom(src => src.ExecutionBillDate.HasValue
                    ? DateOnly.FromDateTime(src.ExecutionBillDate.Value)
                    : (DateOnly?)null))
                .ForMember(e => e.ApprovalBillDate, opts => opts.MapFrom(src => src.ApprovalBillDate.HasValue
                    ? DateOnly.FromDateTime(src.ApprovalBillDate.Value)
                    : (DateOnly?)null))
                .ForMember(e => e.Price, opts => opts.MapFrom(src => src.Price))
                .ForMember(e => e.PriceToMeter, opts => opts.MapFrom(src => src.PriceToMeter))
                .ForMember(e => e.Services, opts => opts.Ignore()); // Нужно обработать отдельно

            CreateMap<Order, OrderShortInfo>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.Name))
                .ForMember(e => e.Status, opts => opts.MapFrom(src => src.IsCompleted ? "Выполнен" : CalculateStatus(src.StartDate, src.Term)))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.Square))
                .ForMember(e => e.ContractNo, opts => opts.MapFrom(src => src.Number))
                .ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.StartDate))
                .ForMember(e => e.AccountNo, opts => opts.MapFrom(src => $"{src.PrepaymentBillNumber} от {src.PrepaymentBillDate}"));
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
