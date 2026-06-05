using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class GestionarVentasView : UserControl
    {
        public GestionarVentasView()
        {
            InitializeComponent();
            DataContext = new ViewModels.GestionarVentasViewModel();
        }
    }
}
