using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Lab1.Views;

public partial class HistogramDisplay : UserControl
{
    public HistogramDisplay()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}