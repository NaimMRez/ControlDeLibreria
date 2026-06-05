using System.Windows.Controls;
using System.Windows.Input;

namespace LibriGest.Views
{
    public partial class RegistrarCompraView : UserControl
    {
        public RegistrarCompraView()
        {
            InitializeComponent();
            DataContext = new ViewModels.RegistrarCompraViewModel();
        }

        private void TextBoxCodigoBarras_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.RegistrarCompraViewModel vm)
                {
                    vm.BuscarProductoPorCodigo();
                    TextBoxCodigoBarras.Focus();
                    TextBoxCodigoBarras.SelectAll();
                }
                e.Handled = true;
            }
        }
    }
}
