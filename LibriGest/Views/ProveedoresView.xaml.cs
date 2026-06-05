using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class ProveedoresView : UserControl
    {
        public ProveedoresView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ProveedoresViewModel();
        }
    }
}
