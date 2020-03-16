namespace InOutManagement.Windows
{
    #region using directive

    using InOutManagement.Commands;

    using log4net;

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    #endregion

    public partial class PromptMessageBox : Window, INotifyPropertyChanged
    {
        private static ILog Logger = LogManager.GetLogger(typeof(PromptMessageBox));

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

        #region 窗体命令       

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

        private void ConfirmInfo(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                this.DialogResult = btn.Name == this.Confirm.Name;
            }
        }

    }
}
