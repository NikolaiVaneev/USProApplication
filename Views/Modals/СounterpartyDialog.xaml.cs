using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.ViewModels.Modals;

namespace USProApplication.Views.Modals
{
    /// <summary>
    /// Логика взаимодействия для СounterpartyDialog.xaml
    /// </summary>
    public partial class СounterpartyDialog : Window
    {
        private CounterpartyDTO? _service;

        public СounterpartyDialog()
        {
            InitializeComponent();
            ((CounterpartyDialogViewModel)DataContext).OnSave += Save;
        }

        private void Save(CounterpartyDTO? service)
        {
            ((CounterpartyDialogViewModel)DataContext).OnSave -= Save;
            _service = service;
            DialogResult = true;
        }

        public bool ShowDialog(CounterpartyDTO counterparty, out CounterpartyDTO? result)
        {
            ((CounterpartyDialogViewModel)DataContext).Counterparty = counterparty;
            result = null;

            if (ShowDialog() != true)
                return false;

            result = _service;
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
                var textBox = sender as TextBox;
                if (Clipboard.ContainsText())
                {
                    var clipboardText = Clipboard.GetText();
                    e.Handled = _regex.IsMatch(clipboardText);
                }
            }
        }
    }
}
