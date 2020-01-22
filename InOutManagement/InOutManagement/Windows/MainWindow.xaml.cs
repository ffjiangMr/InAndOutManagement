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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            instance = this;
            this.maxPath = this.Resources["pathMaximize"] as PathGeometry;
            this.Loaded += (object s, RoutedEventArgs a) =>
            {
                Border windowTitleBorder = (Border)this.Template.FindName("root", this);
                windowTitleBorder.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
                {
                    this.DragMove();
                };
            };
        }       
    }
}
