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

                .ForMember(e => e.NDS, opts => opts.MapFrom(src => src.NDS ?? 0))
                .ForMember(e => e.UsingNDS, opts => opts.MapFrom(src => src.NDS != null))
                .ForMember(e => e.Price, opts => opts.MapFrom(src => src.Price))
                .ForMember(e => e.PriceToMeter, opts => opts.MapFrom(src => src.PriceToMeter))
                .ForMember(e => e.AdditionalService, opts => opts.MapFrom(src => src.AdditionalService))
                .ForMember(e => e.ParentId, opts => opts.MapFrom(src => src.ParentId))
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
                .ForMember(e => e.NDS, opts => opts.MapFrom(src => src.UsingNDS && src.NDS > 0 ? src.NDS : (double?)null))
                .ForMember(e => e.ParentId, opts => opts.MapFrom(src => src.ParentId))
                .ForMember(e => e.AdditionalService, opts => opts.MapFrom(src => src.AdditionalService))
                .ForMember(e => e.Services, opts => opts.Ignore()); // Нужно обработать отдельно

            CreateMap<Order, OrderShortInfo>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.ParentId == null ? src.Name : src.ParentOrder!.Name))
                .ForMember(e => e.Status, opts => opts.MapFrom(src => src.IsCompleted ? "Выполнен" : "В работе"))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.ParentId == null ? src.Address : src.ParentOrder!.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.ParentId == null ? src.Square : src.ParentOrder!.Square))
                .ForMember(e => e.ContractNo, opts => opts.MapFrom(src => src.ParentId == null ? $"{src.Number}" : $"ДС №{src.Number} к {src.ParentOrder!.Number}"))
                .ForMember(e => e.IsMainOrder, opts => opts.MapFrom(src => src.ParentId == null))
                .ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.StartDate));

            CreateMap<OrderDTO, OrderShortInfo>()
                .ForMember(e => e.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(e => e.Name, opts => opts.MapFrom(src => src.ParentId == null ? src.Name : src.ParentOrder!.Name))
                .ForMember(e => e.Status, opts => opts.MapFrom(src => src.IsCompleted ? "Выполнен" : "В работе"))
                .ForMember(e => e.Address, opts => opts.MapFrom(src => src.ParentId == null ? src.Address : src.ParentOrder!.Address))
                .ForMember(e => e.Square, opts => opts.MapFrom(src => src.ParentId == null ? src.Square : src.ParentOrder!.Square))
                .ForMember(e => e.ContractNo, opts => opts.MapFrom(src => src.ParentId == null ? $"{src.Number}" : $"ДС №{src.Number} к {src.ParentOrder!.Number}"))
                .ForMember(e => e.IsMainOrder, opts => opts.MapFrom(src => src.ParentId == null))
                .ForMember(e => e.ContractDate, opts => opts.MapFrom(src => src.StartDate.HasValue
                    ? DateOnly.FromDateTime(src.StartDate.Value)
                    : (DateOnly?)null));
        }
    }
}
