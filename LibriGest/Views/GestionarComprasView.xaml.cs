using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class GestionarComprasView : UserControl
    {
        public GestionarComprasView()
        {
            InitializeComponent();
            DataContext = new ViewModels.GestionarComprasViewModel();
        }
    }
}
