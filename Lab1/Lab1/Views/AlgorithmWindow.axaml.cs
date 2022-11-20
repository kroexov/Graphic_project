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
    private string _selectedAlg = "Ordered";
    private int _bitn = 1;
    private PnmServices _services;
    private Bitmap? _imageToLoad;
    
    private ObservableCollection<string> _algorithms = new ObservableCollection<string>()
    {
        "Ordered",
        "Random",
        "Floyd-Steinberg",
        "Atkinson"
    };
    public AlgorithmWindow()
    {
        InitializeComponent();
    }
    
    public AlgorithmWindow(PnmServices services)
    {
        InitializeComponent();
        _services = services;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    private void RaisePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public string SelectedAlg
    {
        get => _selectedAlg;
        set
        {
            _selectedAlg = value;
            RaisePropertyChanged(nameof(SelectedAlg));
        }
    }
    
    public int Bitn
    {
        get => _bitn;
        set
        {
            _bitn = value;
            RaisePropertyChanged(nameof(Bitn));
        }
    }
    
    public ObservableCollection<string> Algorithms
    {
        get => _algorithms;
    }

    private void OnAlgChosen(object? sender, RoutedEventArgs e)
    {
        SetPath(_services.UseDither(_bitn));
    }
    
    public Bitmap? ImageToLoadPublic
    {
        get => _imageToLoad;
        set
        {
            _imageToLoad = value;
            RaisePropertyChanged(nameof(ImageToLoadPublic));
        }
    }
    
    public void SetPath(string path)
    {
        ImageToLoadPublic = new Bitmap(path);
    }
}