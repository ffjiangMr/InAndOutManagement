namespace InOutManagement.Windows
{

    #region  using directive

    using InOutManagement.Commands;
    using InOutManagement.Common;
    using InOutManagement.Resources.Message;

    using log4net;

    using System;
    using System.ComponentModel;
    using System.Timers;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static ILog Logger = LogManager.GetLogger(typeof(MainWindow));
        private static MainWindow instance;
        public static MainWindow GetInstance()
        {
            return instance;
        }

        #region 设置窗体背景色

        private String windowBackground = "#48c0a3";
        public String WindowBackground
        {
            get
            {
                return this.windowBackground;
            }
            set
            {
                this.windowBackground = value;
                this.OnPropertyChanged("WindowBackground");
            }
        }

        #endregion

        #region 窗体内背景色

        private String windowContentBackground = "#a4e2c6";
        public String WindowContentBackground
        {
            get
            {
                return this.windowContentBackground;
            }
            set
            {
                this.windowContentBackground = value;
                this.OnPropertyChanged("WindowContentBackground");
            }
        }

        #endregion

        #region 窗体命令

        private ICommand maxCommand = MaximizeCommand.GetInstance();

        public ICommand Max
        {
            get
            {
                return this.maxCommand;
            }
            set
            {
                this.maxCommand = value;
                this.OnPropertyChanged("Max");
            }
        }

        private MinimizeCommand minCommand = MinimizeCommand.GetInstance();
        public ICommand Min
        {
            get
            {
                return this.minCommand;
            }
            set
            {
                if (value is MinimizeCommand)
                {
                    this.minCommand = value as MinimizeCommand;
                    this.OnPropertyChanged("Min");
                }
            }
        }

        private CloseCommand closeCommand = CloseCommand.GetInstance();
        public ICommand Shutdown
        {
            get
            {
                return this.closeCommand;
            }
            set
            {
                if (value is CloseCommand)
                {
                    this.closeCommand = value as CloseCommand;
                    this.OnPropertyChanged("Shutdown");
                }
            }
        }

        #endregion

        #region 最大化按钮图像

        private Geometry maxPath;

        public Geometry MaxPath
        {
            get
            {
                return this.maxPath;
            }
            set
            {
                this.maxPath = value;
                this.OnPropertyChanged("MaxPath");
            }
        }

        #endregion

        #region Tips

        public MessageEnum WindowsStatus { get; set; }
        
        public String Tips
        {
            get
            {
                return "Tips: " + MessageResource.ResourceManager.GetString(WindowsStatus.ToString());
            }
            set
            {
                this.OnPropertyChanged("Tips");
            }
        }

        #endregion

        #region 保存窗体位置

        public Rect Position { get; set; }

        #endregion

        #region 按钮命令

        private ICommand queryCommand = QueryCommand.GetInstance();
        public ICommand Query
        {
            get
            {
                return this.queryCommand;
            }
            set
            {
                this.queryCommand = value;
                this.OnPropertyChanged("Query");
            }
        }

        private ICommand inputCommand = InputCommand.GetInstance();
        public ICommand Input
        {
            get
            {
                return this.inputCommand;
            }
            set
            {
                this.inputCommand = value;
                this.OnPropertyChanged("Input");
            }
        }

        private ICommand outputCommand = OutputCommand.GetInstance();
        public ICommand Output
        {
            get
            {
                return this.outputCommand;
            }
            set
            {
                this.outputCommand = value;
                this.OnPropertyChanged("Output");
            }
        }

        #endregion

        #region 控制页面显示

        private Visibility queryVisibility = Visibility.Visible;
        public Visibility QueryVisibility
        {
            get
            {
                return this.queryVisibility;
            }
            set
            {
                this.queryVisibility = value;
                this.OnPropertyChanged("QueryVisibility");
            }
        }

        private Visibility inputVisibility = Visibility.Hidden;
        public Visibility InputVisibility
        {
            get
            {
                return this.inputVisibility;
            }
            set
            {
                this.inputVisibility = value;
                this.OnPropertyChanged("InputVisibility");
            }
        }

        private Visibility outputVisibility = Visibility.Hidden;
        public Visibility OutputVisibility
        {
            get
            {
                return this.outputVisibility;
            }
            set
            {
                this.outputVisibility = value;
                this.OnPropertyChanged("OutputVisibility");
            }
        }

        #endregion 

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Timer timer = new Timer(100);
    }
}
