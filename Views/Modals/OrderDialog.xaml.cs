using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.ViewModels.Modals;

namespace USProApplication.Views.Modals;

/// <summary>
/// Логика взаимодействия для OrderDialog.xaml
/// </summary>
public partial class OrderDialog : Window
{
    private OrderDTO? _order;

    public OrderDialog()
    {
        InitializeComponent();
        ((OrderDialogViewModel)DataContext).OnSave += Save;
    }

    private void Save(OrderDTO? order)
    {
        _order = order;
        DialogResult = true;
    }

    public bool ShowDialog(OrderDTO order, ICollection<DictionaryItem> executors, ICollection<DictionaryItem> clients, ICollection<ServiceItem> services, out OrderDTO? result)
    {
        ((OrderDialogViewModel)DataContext).Executors = new ObservableCollection<DictionaryItem>(executors);
        ((OrderDialogViewModel)DataContext).Customers = new ObservableCollection<DictionaryItem>(clients);
        ((OrderDialogViewModel)DataContext).Services = new ObservableCollection<ServiceItem>(services);
        ((OrderDialogViewModel)DataContext).Order = order;

        var selectedService = order.SelectedServicesIds;
        if (selectedService != null && services.Count > 0)
        {
            foreach (var service in ((OrderDialogViewModel)DataContext).Services)
            {
                if (selectedService.Contains(service.Id))
                    service.IsChecked = true;
            }
        }

        result = null;

        if (ShowDialog() != true)
            return false;

        result = _order;
        return true;
    }

    private static readonly Regex _regex = new Regex("[^0-9]+"); // Только цифры

    // Обработка ввода с клавиатуры
    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = _regex.IsMatch(e.Text);
    }

    // Обработка вставки текста
    private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Paste)
        {
            if (Clipboard.ContainsText())
            {
                var clipboardText = Clipboard.GetText();
                e.Handled = _regex.IsMatch(clipboardText);
            }
        }
    }
}
