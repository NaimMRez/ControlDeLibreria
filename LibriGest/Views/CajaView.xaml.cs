using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class CajaView : UserControl
    {
        public CajaView()
        {
            InitializeComponent();
            DataContext = new ViewModels.CajaViewModel();
        }
    }
}
