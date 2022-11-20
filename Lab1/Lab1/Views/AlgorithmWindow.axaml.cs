using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Lab1.ViewModels;

public partial class AlgorithmWindow : Window
{
    public AlgorithmWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}