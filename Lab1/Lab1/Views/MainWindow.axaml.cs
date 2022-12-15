using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
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

        private void ShowFilter(object? sender, RoutedEventArgs e)
        {
            var fullFileName = _mvm.ApplySelectedFiler();
            ImageDisplayViewModel viewModel = new ImageDisplayViewModel();
            if (!fullFileName.Equals(String.Empty) )
            {
                viewModel.SetPath(fullFileName);
            }
            FilterCheckWindow filterCheckWindow = new FilterCheckWindow()
            {
                DataContext = viewModel
            };
            filterCheckWindow.Show();
        }
    }
}