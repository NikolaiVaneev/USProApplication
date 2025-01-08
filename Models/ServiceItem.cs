using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace USProApplication.Models
{
    public class ServiceItem : ReactiveObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [Reactive] public bool IsChecked { get; set; }
        public decimal Price { get; set; } = 0;
    }
}