
using System.IO;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Lab1.ViewModels;

public class HistogramDisplayViewModel : ViewModelBase
{
    #region Private fields
    
    private Bitmap? _channel1Histogram;
    
    private Bitmap? _channel2Histogram;
    
    private Bitmap? _channel3Histogram;

    #endregion

    #region Public methods

    public void SetPathForChannel1(string path)
    {
        using var fileStream = File.OpenRead(path);
        Channel1Histogram = new Bitmap(fileStream);
    }
    
    public void SetPathForChannel2(string path)
    {
        using var fileStream = File.OpenRead(path);
        Channel2Histogram = new Bitmap(fileStream);
    }
    
    public void SetPathForChannel3(string path)
    {
        using var fileStream = File.OpenRead(path);
        Channel3Histogram = new Bitmap(fileStream);
    }

    #endregion

    #region Public properties

    public Bitmap? Channel1Histogram
    {
        get => _channel1Histogram;
        private set => this.RaiseAndSetIfChanged(ref _channel1Histogram, value);
    }
    
    public Bitmap? Channel2Histogram
    {
        get => _channel2Histogram;
        private set => this.RaiseAndSetIfChanged(ref _channel2Histogram, value);
    }
    
    public Bitmap? Channel3Histogram
    {
        get => _channel3Histogram;
        private set => this.RaiseAndSetIfChanged(ref _channel3Histogram, value);
    }

    #endregion
}