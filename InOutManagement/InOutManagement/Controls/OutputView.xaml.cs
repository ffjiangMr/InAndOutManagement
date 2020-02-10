using InOutManagement.Windows;

using System.Windows.Controls;

namespace InOutManagement.Controls
{
    /// <summary>
    /// Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutputView : UserControl
    {
        public OutputView()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += delegate
            {
                this.Initial();
                this.mainWindow = MainWindow.GetInstance();
            };
            this.Unloaded += delegate
            {
                this.timer?.Stop();
                this.timer?.Dispose();
            };
        }
    }
}
