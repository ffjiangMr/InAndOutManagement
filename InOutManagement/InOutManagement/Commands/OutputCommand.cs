namespace InOutManagement.Commands
{
    #region using directive

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;

    #endregion

    public sealed class OutputCommand : ICommand
    {
        #region  singal instance

        private static OutputCommand instance;

        public static OutputCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new OutputCommand();
            }
            return instance;
        }

        #endregion

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = MainWindow.GetInstance();
            window.QueryVisibility = Visibility.Hidden;
            window.InputVisibility = Visibility.Hidden;
            window.OutputVisibility = Visibility.Visible;
        }
    }
}
