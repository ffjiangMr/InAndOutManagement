namespace InOutManagement.Commands
{
    #region using directive

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;

    #endregion 

    public sealed class QueryCommand : ICommand
    {

        #region  singal instance

        private static QueryCommand instance;

        public static QueryCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new QueryCommand();
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
            window.QueryVisibility = Visibility.Visible;
            window.InputVisibility = Visibility.Hidden;
            window.OutputVisibility = Visibility.Hidden;
        }
    }
}
