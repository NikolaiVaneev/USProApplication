﻿using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.ViewModels.Modals;

namespace USProApplication.Views.Modals
{
    /// <summary>
    /// Логика взаимодействия для AdditionalOrderDialog.xaml
    /// </summary>
    public partial class AdditionalOrderDialog : Window
    {
        private OrderDTO? _order;

        public AdditionalOrderDialog()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<OrderDialogViewModel>()
                  ?? throw new InvalidOperationException("Не удалось получить экземпляр OrderDialogViewModel.");
            ((OrderDialogViewModel)DataContext).OnSave += Save;
        }

        private void Save(OrderDTO? order)
        {
            _order = order;
            DialogResult = true;
        }

        public bool ShowDialog(OrderDTO order, ICollection<ServiceItem> services, out OrderDTO? result)
        {
            var viewModel = (OrderDialogViewModel)DataContext;

            viewModel.Order = order;
            viewModel.PriceToMeter = order.PriceToMeter ?? 0;
            viewModel.TotalPrice = order.Price ?? 0;


            viewModel.InitializeServices(services, order.SelectedServicesIds);

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((OrderDialogViewModel)DataContext).OnSave -= Save;
        }
    }
}
