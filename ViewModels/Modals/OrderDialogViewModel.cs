using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using USProApplication.Models;
using USProApplication.Utils;

namespace USProApplication.ViewModels.Modals;

public class OrderDialogViewModel(IDocCreator docCreator) : ReactiveObject
{
    [Reactive] public OrderDTO? Order { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Executors { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Customers { get; set; }
    [Reactive] public ObservableCollection<ServiceItem>? Services { get; set; } = [];
    [Reactive] public int SelectedServicesCount { get; set; }

    [Reactive] public decimal TotalPrice { get; set; } = 0.0m;
    [Reactive] public decimal PriceToMeter { get; set; } = 0.0m;

    [Reactive] public bool NeedStamp { get; set; } = true;

    public event Action<OrderDTO?>? OnSave;

    private DelegateCommand? apply;
    public DelegateCommand Apply => apply ??= new DelegateCommand(() =>
    {
        Order!.SelectedServicesIds = Services?
            .Where(s => s.IsChecked)
            .Select(s => s.Id)
            .ToList();

        Order.PriceToMeter = PriceToMeter;
        Order.Price = TotalPrice;

        OnSave?.Invoke(Order);
    }, () => !string.IsNullOrWhiteSpace(Order?.Number));

    public void InitializeServices(ICollection<ServiceItem> services, ICollection<Guid>? selectedServiceIds)
    {
        Services = new ObservableCollection<ServiceItem>(services);

        if (selectedServiceIds != null)
        {
            foreach (var service in Services)
            {
                service.IsChecked = selectedServiceIds.Contains(service.Id);
            }
        }

        foreach (var service in Services)
        {
            service.WhenAnyValue(s => s.IsChecked)
                .Subscribe(_ => UpdateSelectedServicesCount());
        }

        UpdateSelectedServicesCount();
    }

    private DelegateCommand? calculatePrice;
    public DelegateCommand CalculatePrice => calculatePrice ??= new DelegateCommand(() =>
    {
        if (Order == null) return;

        int square = Order.Square;
        decimal priceToMeter = 0;
        decimal fullPrice = 0;


        var usingServices = Services?.Where(s => s.IsChecked).ToList();
        if (usingServices != null && usingServices.Count > 0)
        {
            foreach (var service in usingServices)
            {
                priceToMeter += service.Price;
            }

            fullPrice = priceToMeter * square;

            if (Order.UsingNDS && Order.NDS > 0)
            {
                priceToMeter = Math.Round(priceToMeter * (Order.NDS + 100) / 100, 2);
                fullPrice = Math.Round(fullPrice * (Order.NDS + 100) / 100, 2);
            }

            PriceToMeter = priceToMeter;
            TotalPrice = fullPrice;
        }

    });

    private void UpdateSelectedServicesCount()
    {
        SelectedServicesCount = Services?.Count(s => s.IsChecked) ?? 0;
    }

    private AsyncCommand? _createContractCommand;
    public AsyncCommand CreateContractCommand => _createContractCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            if (Order!.ParentId == null)
            {
                await docCreator.CreateContractAsync(Order!, NeedStamp);
            }
            else
            {
                await docCreator.CreateAdditionalContractAsync(Order!, NeedStamp);
            }
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }, () => !string.IsNullOrWhiteSpace(Order?.Name) || !string.IsNullOrWhiteSpace(Order?.ParentOrder?.Name));

    private AsyncCommand? _createPrepaymentInvoiceCommand;
    public AsyncCommand CreatePrepaymentInvoiceCommand => _createPrepaymentInvoiceCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreatePaymentInvoiceAsync(Order!, PaymentInvioceTypes.Prepayment, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
    }, () => Order!.PrepaymentPercent > 0 && !string.IsNullOrWhiteSpace(Order!.PrepaymentBillNumber) && Order!.PrepaymentBillDate != null);

    private AsyncCommand? _createExecutionInvoiceCommand;
    public AsyncCommand CreateExecutionInvoiceCommand => _createExecutionInvoiceCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreatePaymentInvoiceAsync(Order!, PaymentInvioceTypes.Execution, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }, () => Order!.ExecutionPercent > 0 && !string.IsNullOrWhiteSpace(Order!.ExecutionBillNumber) && Order!.ExecutionBillDate != null);

    private AsyncCommand? _createApprovalInvoiceCommand;
    public AsyncCommand CreateApprovalInvoiceCommand => _createApprovalInvoiceCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreatePaymentInvoiceAsync(Order!, PaymentInvioceTypes.Approval, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }, () => Order!.ApprovalPercent > 0 && !string.IsNullOrWhiteSpace(Order!.ApprovalBillNumber) && Order!.ApprovalBillNumber != null);

    private AsyncCommand? _createContractInvoiceCommand;
    public AsyncCommand CreateContractInvoiceCommand => _createContractInvoiceCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreateContractInvoiceAsync(Order!, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }, () => Order!.CustomerId != null && Order!.ExecutorId != null && !string.IsNullOrWhiteSpace(Order!.Address) && !string.IsNullOrWhiteSpace(Order!.Number) && !string.IsNullOrWhiteSpace(Order.AdditionalService));

    private AsyncCommand? _createActCommand;
    public AsyncCommand CreateActCommand => _createActCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreateActAsync(Order!, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
    }, () => ((Order!.ParentId == null && Order!.CustomerId != null && Order!.ExecutorId != null) || Order!.ParentId != null)
            && !string.IsNullOrWhiteSpace(Order.Number)
            && Order.StartDate != null
            && Order.СompletionDate != null);

    private AsyncCommand? _createUPDCommand;
    public AsyncCommand CreateUPDCommand => _createUPDCommand ??= new AsyncCommand(async () =>
    {
        try
        {
            await docCreator.CreateUPDAsync(Order!, NeedStamp);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка создания документа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }, () => ((Order!.ParentId == null && Order!.CustomerId != null && Order!.ExecutorId != null) || Order!.ParentId != null)
            && !string.IsNullOrWhiteSpace(Order.Number)
            && Order.StartDate != null
            && Order.СompletionDate != null);
}