using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace USProApplication.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MenuItemLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var dependencyObject = Mouse.Captured as DependencyObject;

        while (dependencyObject != null)
        {
            if (dependencyObject is ScrollBar) return;
            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
        }

        MenuToggleButton.IsChecked = false;
    }
    
}