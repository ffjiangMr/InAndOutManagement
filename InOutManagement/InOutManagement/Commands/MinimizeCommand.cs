namespace InOutManagement.Commands
{
    #region using directive

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;

    #endregion

    public sealed class MinimizeCommand : ICommand
    {
        #region  singal instance

        private static MinimizeCommand instance;

        public static MinimizeCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new MinimizeCommand();
            }
            return instance;
        }

        #endregion

        private MinimizeCommand()
        {

        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindow.GetInstance().WindowState = WindowState.Minimized;
        }
    }
}
