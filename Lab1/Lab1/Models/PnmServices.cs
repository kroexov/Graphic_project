using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Lab1.TypeFileImg;

namespace Lab1.Models;

public class PnmServices: IPnmServices
{
    #region Private fields

    private TypeFile _typeFile;
    private byte[] _bytes;
    private string _filePath;
    private Pnm _fileImg;
    private bool _isGenerated;
    private DitheringServices _ditheringServices = new DitheringServices();
    private string _selectedPath;

    #endregion

    #region Public methods

    public string ReadFile(string filePath, bool[] channels, ColorSpace colorSpace = ColorSpace.Rgb)
    {
        _isGenerated = false;
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }
        
        _bytes = File.ReadAllBytes(filePath);
        _typeFile = FindTypeFile();
        _filePath = filePath;
        _fileImg = FindPnmImg(colorSpace);
        _fileImg.SetColorChannel(channels);
        _selectedPath = RefreshImage();

        return _selectedPath;
    }

    public void AssignGamma(double newGamma)
    {
        if (_fileImg != null)
        {
            _fileImg.SetGammaСoefficient(newGamma);
        }   
    }
    
    public void ConvertGamma(double newGamma)
    {
        if (_fileImg != null)
        {
            _fileImg.ConvertGamma(newGamma);
        }
    }

    public byte[] SaveFile()
    {
        if (_isGenerated)
        {
            return File.ReadAllBytes(_selectedPath);
        }
        return _fileImg.SaveFile(_bytes);
    }

    public string RefreshImage()
    {
        if (_fileImg == null)
        {
            return String.Empty;
        }
        var fileName = Path.GetFileName(_filePath);
        fileName = fileName.Substring(0, fileName.Length - 3) + "bmp";
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + fileName;
        var test  = _fileImg.CreateBitmap();
        test.Save(fullFileName, ImageFormat.Bmp);
        return fullFileName;
    }

    public string UseDither(int bitn, string selectedAlgorithm)
    {
        OnAlgChosen?.Invoke();
        switch (selectedAlgorithm)
        {
            case "Random":
                return _ditheringServices.RandomAlgorithm(_selectedPath, bitn);
            case "Floyd-Steinberg":
                return _ditheringServices.FloydSteinbergAlgorithm(_selectedPath, bitn);
            case "Atkinson":
                return _ditheringServices.AtkinsonAlgorithm(_selectedPath, bitn);
            default:
                return _ditheringServices.OrderedAlgorithm(_selectedPath, bitn);
        }
    }

    public void ApplyDithering()
    {
        _fileImg.ChangeData();
        DitheredApplied?.Invoke(RefreshImage());
    }
    
    public void ChangeColorSpace(ColorSpace newColorSpace)
    {
        if (_fileImg != null)
        {
            _fileImg.ConvertColor(newColorSpace);
        }
    }

    public void ChangeColorChannel(bool[] newColorChannel)
    {
        if (_fileImg != null)
        {
            _fileImg.SetColorChannel(newColorChannel);
        }
    }

    public string CreateGradient(int width, int height)
    {
        var header = "P5\n" + width + " " + height + "\n255\n";
        var fileGradient = Encoding.UTF8.GetBytes(header);
        Array.Resize(ref fileGradient, height * width + header.Length);
        double step = 1.0 / width;

        double currentValue = 0;

        for (var x = 0; x < width; x++)
        {
            var value = currentValue * 255;
            for (var y = 0; y < height; y++)
            {
                fileGradient[y * width + x + header.Length] = (byte)Math.Round(value);
            }
            currentValue += step;
        }
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "gradient.pgm";
        
        File.WriteAllBytes(fullFileName , fileGradient);
        return fullFileName;
    }

    public string CreateGradientImage(int width, int height)
    {
        _isGenerated = true;
        var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        double step = 1.0 / width;

        double currentValue = 0;

        for (var x = 0; x < width; x++)
        {
            var value = currentValue * 255;
            for (var y = 0; y < height; y++)
            {
                Color newColor = Color.FromArgb((byte)Math.Round(value),
                    (byte)Math.Round(value),
                    (byte)Math.Round(value));

                image.SetPixel(x, y, newColor);
            }
            currentValue += step;
        }
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "gradient.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);
        _selectedPath = fullFileName;
        return fullFileName;
    }

    #endregion
    
    #region Private methods

    private TypeFile FindTypeFile()
    {
        var res = "";
        for (var i = 0; i < 16; i++)
        {
            res += Convert.ToChar(_bytes[i]);
        }

        if (res[0] == 'P')
        {
            if (res[1] == '5')
                return TypeFile.P5;
            
            if (res[1] == '6')
                return TypeFile.P6;
        }
        else if (res[1] == 'P' && res[1] == 'N' && res[1] == 'G')
        {
            return TypeFile.Png;
        }

        return TypeFile.IsFalseFile;
    }

    private Pnm FindPnmImg(ColorSpace colorSpace)
    {
        switch (_typeFile)
        {
            case TypeFile.P5:
            {
                return new P5(_bytes);
            }
            case TypeFile.P6:
            {
                return new P6(_bytes, colorSpace);
            }
        }

        return null;
    }

    #endregion
    
    #region Events

    public event Action? OnAlgChosen;
    public event Action<string>? ModelErrorHappened;

    public event Action<string>? DitheredApplied;

    #endregion
}