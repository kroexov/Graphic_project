using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Lab1.Views;

public partial class FilterCheckWindow : Window
{
    public FilterCheckWindow()
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