using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        return RefreshImage();
    }

    public byte[] SaveFile()
    {
        if (_isGenerated)
        {
            var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
            pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
            var fullFileName = pathSaveFile + "\\imgFiles\\" + "gradient.bmp";
            return File.ReadAllBytes(fullFileName);
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

    public string UseDither(int bitn)
    {
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "image.psd-_12_.bmp";

        return _ditheringServices.OrderedAlgorithm(fullFileName, bitn);
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

    public event Action<string>? ModelErrorHappened;

    #endregion
}