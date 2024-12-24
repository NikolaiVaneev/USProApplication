using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace USProApplication.Utils;

public static class DataGridDoubleClickBehavior
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(DataGridDoubleClickBehavior),
            new PropertyMetadata(null, OnCommandChanged));

    public static void SetCommand(UIElement element, ICommand value)
    {
        element.SetValue(CommandProperty, value);
    }

    public static ICommand GetCommand(UIElement element)
    {
        return (ICommand)element.GetValue(CommandProperty);
    }

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid dataGrid)
        {
            if (e.OldValue != null)
            {
                dataGrid.MouseDoubleClick -= DataGrid_MouseDoubleClick;
            }

            if (e.NewValue != null)
            {
                dataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            }
        }
    }

    private static void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
        {
            var command = GetCommand(dataGrid);
            if (command != null && command.CanExecute(dataGrid.SelectedItem))
            {
                command.Execute(dataGrid.SelectedItem);
            }
        }
    }
}