namespace InOutManagement.Commands
{

    #region using directive

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;

    #endregion 

    public class InputCommand : ICommand
    {
        #region  singal instance

        private static InputCommand instance;

        public static InputCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new InputCommand();
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
            window.InputVisibility = Visibility.Visible;
            window.OutputVisibility = Visibility.Hidden;
        }
    }
}
