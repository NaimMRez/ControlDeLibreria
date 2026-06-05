using System.Windows.Controls;
using System.Windows.Input;

namespace LibriGest.Views
{
    public partial class AjustesInventarioView : UserControl
    {
        public AjustesInventarioView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AjustesInventarioViewModel();
        }

        private void TextBoxCodigoBarras_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.AjustesInventarioViewModel vm)
                {
                    vm.BuscarProducto();
                    TextBoxCodigoBarras.Focus();
                    TextBoxCodigoBarras.SelectAll();
                }
                e.Handled = true;
            }
        }
    }
}
