using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Lab1.ViewModels;

public partial class AlgorithmWindow : Window
{
    public AlgorithmWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private ObservableCollection<string> _spaces = new ObservableCollection<string>()
    {
        "Ordered (8x8)",
        "HSL",
        "HSV",
        "YCbCr601",
        "YCbCr709",
        "YСoCg",
        "CMY"
    };
}