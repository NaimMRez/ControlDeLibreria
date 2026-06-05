using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new ViewModels.DashboardViewModel();
        }
    }
}
