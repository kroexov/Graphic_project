using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Lab1.ViewModels;

public class ImageDisplayViewModel : ViewModelBase
{
    public ImageDisplayViewModel(string path)
    {
        ImageToLoad = new Bitmap(path);
    }
    private Bitmap? ImageToLoad;
    public Bitmap? ImageToLoadPublic
    {
        get => ImageToLoad;
        private set => this.RaiseAndSetIfChanged(ref ImageToLoad, value);
    }
}