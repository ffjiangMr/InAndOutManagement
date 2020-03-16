namespace InOutManagement.Windows
{
    #region using directive

    using log4net;

    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;

    #endregion

    public sealed partial class ProcessBar : Window, INotifyPropertyChanged
    {
        private static ILog Logger = LogManager.GetLogger(typeof(ProcessBar));

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

        #endregion·

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

        #region Message

        private String message = String.Empty;
        public String Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
                this.OnPropertyChanged("Message");
            }
        }

        #endregion

        #region 进度

        private Double schedule = 0;

        public Double Schedule
        {
            get
            {
                return this.schedule;
            }
            set
            {
                this.schedule = value;
                this.OnPropertyChanged("Schedule");
            }
        }

        #endregion

        public void UpdateProcess(String msg, Double pro)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Message = msg;
                this.Schedule = pro;
            }));
        }

        public void ShowSchedule(Action action)
        {
            Task task = new Task(action);
            task.Start();
            this.ShowDialog();
        }

        public void CloseDialog()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        public Boolean ShowMessage(String msg)
        {
            Boolean result = false;
            this.Message = msg;
            result = this.ShowDialog() == true;
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            var temp = this.PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
