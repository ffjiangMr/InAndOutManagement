using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InOutManagement.Windows
{
    /// <summary>
    /// PromptMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class PromptMessageBox : Window, INotifyPropertyChanged
    {
        public PromptMessageBox()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += delegate
            {
                Logger.Info("Func in.");
                Logger.Debug("主窗体加载完成");
                Border windowTitleBorder = (Border)this.Template.FindName("root", this);
                windowTitleBorder.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
                {
                    this.DragMove();
                };
                Logger.Info("Func out.");
            };
        }        
    }
}
