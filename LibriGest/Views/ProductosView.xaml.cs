using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class ProductosView : UserControl
    {
        public ProductosView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ProductosViewModel();
        }
    }
}
