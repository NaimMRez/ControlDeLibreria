using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class ConfiguracionView : UserControl
    {
        public ConfiguracionView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ConfiguracionViewModel();
        }
    }
}
