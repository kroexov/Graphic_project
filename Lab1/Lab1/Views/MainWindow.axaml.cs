using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
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
            mvm.HeightChanged += MvmOnHeightChanged;
            mvm.WidthChanged += MvmOnWidthChanged;
        }

        private void MvmOnWidthChanged(double obj)
        {
            WidthField.Value = obj;
        }

        private void MvmOnHeightChanged(double obj)
        {
            HeightField.Value = obj;
        }

        private void MvmOnOnErrorHappened(string error)
        {
            ErrorWindow errorWindow = new ErrorWindow(error);
            errorWindow.Show();
        }
    }
}