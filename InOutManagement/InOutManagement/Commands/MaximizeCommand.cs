namespace InOutManagement.Commands
{
    #region using directive

    using InOutManagement.Windows;

    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    #endregion

    public sealed class MaximizeCommand : ICommand
    {

        #region  singal instance

        private static MaximizeCommand instance;

        public static MaximizeCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new MaximizeCommand();
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
            if (parameter is MainWindow)
            {
                var window = MainWindow.GetInstance();
                window.Position = new Rect(window.Left, window.Top, window.Width, window.Height);
                window.Left = 0;
                window.Top = 0;                
                window.Width = SystemParameters.WorkArea.Width;
                window.Height = SystemParameters.WorkArea.Height;
                window.Max = RestoreCommand.GetInstance();
                window.MaxPath = window.Resources["pathRestore"] as PathGeometry;
            }
        }
    }
}
