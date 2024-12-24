using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using USProApplication.Models;
using USProApplication.Utils;

namespace USProApplication.ViewModels.Modals;

public partial class ServiceDialogViewModel : ReactiveObject
{
    [Reactive] public Service? Service { get; set; }

    public event Action<Service?>? OnSave;

    private DelegateCommand? apply;
    public DelegateCommand Apply => apply ??= new DelegateCommand(() =>
    {
        OnSave?.Invoke(Service);

    }, () => !string.IsNullOrWhiteSpace(Service?.Name) 
          && !string.IsNullOrWhiteSpace(Service?.Abbreviation));
}