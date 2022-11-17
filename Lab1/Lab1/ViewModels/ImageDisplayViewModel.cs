using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Lab1.ViewModels;

public class ImageDisplayViewModel : ViewModelBase
{
    public ImageDisplayViewModel()
    {
        
    }
    
    private Bitmap? ImageToLoad;

    public void SetPath(string path)
    {
        ImageToLoadPublic = new Bitmap(path);
    }
    public Bitmap? ImageToLoadPublic
    {
        get => ImageToLoad;
        private set => this.RaiseAndSetIfChanged(ref ImageToLoad, value);
    }
}