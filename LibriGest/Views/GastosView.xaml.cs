using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class GastosView : UserControl
    {
        public GastosView()
        {
            InitializeComponent();
            DataContext = new ViewModels.GastosViewModel();
        }
    }
}
