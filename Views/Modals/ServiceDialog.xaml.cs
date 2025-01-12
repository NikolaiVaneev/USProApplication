using System.Windows;
using USProApplication.Models;
using USProApplication.ViewModels.Modals;

namespace USProApplication.Views.Modals
{
    public partial class ServiceDialog : Window
    {
        private Service? _service;

        public ServiceDialog()
        {
            InitializeComponent();
            ((ServiceDialogViewModel)DataContext).OnSave += Save;
        }

        private void Save(Service? service)
        {
            ((ServiceDialogViewModel)DataContext).OnSave -= Save;
            _service = service;
            DialogResult = true;
        }

        public bool ShowDialog(Service service, out Service? result)
        {
            ((ServiceDialogViewModel)DataContext).Service = service;
            result = null;

            if (ShowDialog() != true)
                return false;

            result = _service;
            return true;
        }
    }
}