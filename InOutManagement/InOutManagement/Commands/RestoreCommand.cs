namespace InOutManagement.Commands
{
    #region  using directive 

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion
    public sealed class RestoreCommand : ICommand
    {
        #region  singal instance

        private static RestoreCommand instance;

        public static RestoreCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new RestoreCommand();
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
            window.Left = window.Position.Left;
            window.Top = window.Position.Top;
            window.Width = window.Position.Width;
            window.Height = window.Position.Height;
            window.Max = MaximizeCommand.GetInstance();
            window.MaxPath = window.Resources["pathMaximize"] as PathGeometry;
        }
    }
}
