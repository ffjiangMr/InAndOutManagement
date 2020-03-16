using InOutManagement.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InOutManagement.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            instance = this;            
            this.WindowsStatus = MessageEnum.NONE;
            this.maxPath = this.Resources["pathMaximize"] as PathGeometry;
            this.Loaded += delegate
            {
                Logger.Info("Func in.");
                Logger.Debug("主窗体加载完成");                
                Border windowTitleBorder = (Border)this.Template.FindName("root", this);
                windowTitleBorder.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
                {
                    this.DragMove();
                };
                this.timer.Elapsed += delegate
                {                    
                    this.Tips = String.Empty;
                };
                this.timer?.Start();
                Logger.Debug("启动定时器");
                this.WindowsStatus = MessageEnum.Initialed;
                Logger.Info("Func out.");
            };
            this.Unloaded += delegate
            {
                Logger.Info("Func in.");
                Logger.Debug("主窗体卸载完成");
                this.timer?.Stop();
                this.timer.Dispose();
                Logger.Debug("关闭定时器");
                Logger.Info("Func out.");
            };
        }
    }
}
