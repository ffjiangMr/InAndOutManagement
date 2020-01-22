namespace InOutManagement.Commands
{

    #region using directive 

    using InOutManagement.Windows;

    using System;
    using System.Windows.Input;

    #endregion

    public sealed class CloseCommand : ICommand
    {

        #region  singal instance

        private static CloseCommand instance;

        public static CloseCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new CloseCommand();
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
            MainWindow.GetInstance().Close();
        }
    }
}
