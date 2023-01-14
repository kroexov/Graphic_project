using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Lab1.ViewModels;

public class ImageDisplayViewModel : ViewModelBase
{
    #region Private fields

    private Bitmap? _imageToLoad;

    #endregion

    #region Public methods

    public void SetPath(string path)
    {
        ImageToLoadPublic = new Bitmap(path);
    }

    #endregion

    #region Public properties

    public Bitmap? ImageToLoadPublic
    {
        get => _imageToLoad;
        private set => this.RaiseAndSetIfChanged(ref _imageToLoad, value);
    }

    #endregion
}