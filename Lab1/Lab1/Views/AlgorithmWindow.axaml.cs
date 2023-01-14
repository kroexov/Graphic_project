using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Lab1.Models;
using ReactiveUI;

namespace Lab1.ViewModels;

public partial class AlgorithmWindow : Window
{
    public AlgorithmWindow(AlgorithmWindowViewModel algorithmWindowViewModel)
    {
        InitializeComponent();
        DataContext = algorithmWindowViewModel;
    }

    public AlgorithmWindow()
    {
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}