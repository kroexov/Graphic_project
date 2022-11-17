using System.Collections.Generic;
using Avalonia.Controls;
using Lab1.ViewModels;

namespace Lab1.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mvm;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(MainWindowViewModel mvm)
        {
            InitializeComponent();
            this._mvm = mvm;
            mvm.OnErrorHappened += MvmOnOnErrorHappened;
        }

        private void MvmOnOnErrorHappened(string error)
        {
            ErrorWindow errorWindow = new ErrorWindow(error);
            errorWindow.Show();
        }
    }
}