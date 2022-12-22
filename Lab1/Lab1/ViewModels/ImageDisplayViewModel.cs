using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Lab1.ViewModels;

public class ImageDisplayViewModel : ViewModelBase
{
    #region Private fields

    private CroppedBitmap? _imageToLoad;

    #endregion

    #region Public methods

    public void SetPath(string path)
    {
        CroppedBitmap image = new CroppedBitmap(new Bitmap(path), PixelRect.Parse("0 0 500 500"));
        ImageToLoadPublic = image;
    }

    public void SetImage(CroppedBitmap image)
    {
        ImageToLoadPublic = image;
    }

    #endregion

    #region Public properties

    public CroppedBitmap? ImageToLoadPublic
    {
        get => _imageToLoad;
        private set => this.RaiseAndSetIfChanged(ref _imageToLoad, value);
    }

    #endregion
}