using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace USProApplication.Views.Modals
{
    /// <summary>
    /// Логика взаимодействия для СounterpartyDialog.xaml
    /// </summary>
    public partial class СounterpartyDialog : Window
    {
        public СounterpartyDialog()
        {
            InitializeComponent();
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
