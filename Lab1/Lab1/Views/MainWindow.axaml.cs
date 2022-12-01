using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Lab1.ViewModels;

namespace Lab1.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mvm;
        private bool _firstTime = true;
        private double _x;
        private double _y;
        
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

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_firstTime)
            {
                _firstTime = false;
                var point = e.GetCurrentPoint(sender as ImageDisplay);
                _x = point.Position.X;
                _y = point.Position.Y;
                _mvm.Point1 = _x.ToString(CultureInfo.InvariantCulture) + " " + _y.ToString(CultureInfo.InvariantCulture);
                Point1.Text = _x.ToString("F1", CultureInfo.InvariantCulture) + " " + _y.ToString("F1", CultureInfo.InvariantCulture);
            }

            else
            {
                _firstTime = true;
                var point = e.GetCurrentPoint(sender as ImageDisplay);
                _x = point.Position.X;
                _y = point.Position.Y;
                _mvm.Point2 = _x.ToString(CultureInfo.InvariantCulture) + " " + _y.ToString(CultureInfo.InvariantCulture);
                Point2.Text = _x.ToString("F1", CultureInfo.InvariantCulture) + " " + _y.ToString("F1", CultureInfo.InvariantCulture);
            }
        
        }
    }
}