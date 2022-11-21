using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Lab1.Views;

public partial class ImageDisplay : UserControl
{
    private bool _firstTime = true;
    private double _x;
    private double _y;
    public ImageDisplay()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_firstTime)
        {
            _firstTime = false;
            var point = e.GetCurrentPoint(sender as Image);
            _x = point.Position.X;
            _y = point.Position.Y;
        }

        else
        {
            var point = e.GetCurrentPoint(sender as Image);
            //DrawFunction(_x, _y, point.Position.X, point.Position.Y);
        }
        
    }
}