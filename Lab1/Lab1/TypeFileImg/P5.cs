using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P5 : Pnm
{

    #region Constructor

    public P5(byte[] bytes) : base(bytes)
    {
        
    }

    #endregion

    #region Public methods

    public override Bitmap CreateBitmap()
    {
        var image = new Bitmap(Header.Width, Header.Height, PixelFormat.Format24bppRgb);

        for (var x = 0; x < Header.Width; x++)
        {
            for (var y = 0; y < Header.Height; y++)
            {
                var valueColor = 255 * Data[GetCoordinates(x, y)];
                Color newColor = Color.FromArgb((byte)Math.Round(valueColor), (byte)Math.Round(valueColor), (byte)Math.Round(valueColor));
                image.SetPixel(x, y, newColor);
            }
        }
        return image;
    }

    public override void ConvertColor(ColorSpace colorSpace)
    {
    }

    public override void SetColorChannel(bool[] newColorChannel)
    {
    }

    public override byte[] SaveFile(byte[] origFile)
    {
        var saveFile = new byte[Header.Height * Header.Width * Header.PixelSize + _index];

        for (var i = 0; i < _index; i++)
            saveFile[i] = origFile[i];
        
        for (var i = 0; i < Header.Height * Header.Width * Header.PixelSize; i++)
            saveFile[i + _index] = (byte)Math.Round(Data[i] * 255);

        return saveFile;
    }
    
    public override string CreateColorHistogram(double valueIgnore)
    {
        var histogramFirstChannel = new int[256];
        var histogramSecondChannel = new int[256];
        var histogramThirdChannel = new int[256];

        for (var i = 0; i < Header.PixelSize * Header.Height * Header.Width; i += 3)
        {
            var firstChannel = Convert.ToInt32(Math.Round(Data[i] * 255));
            var secondChannel = Convert.ToInt32(Math.Round(Data[i] * 255));
            var thirdChannel = Convert.ToInt32(Math.Round(Data[i] * 255));
            histogramFirstChannel[firstChannel]++;
            histogramSecondChannel[secondChannel]++;
            histogramThirdChannel[thirdChannel]++;
        }

        var maxValue = histogramFirstChannel.Max();
        if (maxValue < histogramSecondChannel.Max())
            maxValue = histogramSecondChannel.Max();
        if (maxValue < histogramThirdChannel.Max())
            maxValue = histogramThirdChannel.Max();
        
        var image = new Bitmap(256*3, maxValue, PixelFormat.Format24bppRgb);
        for (var x = 0; x < 256; x++)
        {
            for (var y = maxValue - histogramFirstChannel[x]; y < maxValue; y++)
            {
                image.SetPixel(3*x, y, Color.Red);
            }
            for (var y = maxValue - histogramSecondChannel[x]; y < maxValue; y++)
            {
                image.SetPixel(3*x + 1, y, Color.Lime);
            }
            for (var y = maxValue - histogramThirdChannel[x]; y < maxValue; y++)
            {
                image.SetPixel(3*x + 2, y, Color.Blue);
            }
        }
        var ignoreValuePixel = 0;
        if (valueIgnore != 0)
        {
            ignoreValuePixel = Convert.ToInt32(Math.Round(256 * valueIgnore)) - 1;
        }

        var leftOffset = ignoreValuePixel;
        for (var i = ignoreValuePixel; i < 256 && histogramFirstChannel[i] == 0; i++)
        {
            leftOffset = i;
        }

        var rightOffset = 255 - ignoreValuePixel;
        for (var i = 255 - ignoreValuePixel; i >= 0 && histogramFirstChannel[i] == 0; i--)
        {
            rightOffset = i;
        }

        var usedValue = rightOffset - leftOffset;

        if (usedValue > 0)
        {
            AutoContrast(rightOffset, leftOffset);
        }
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\SystemImage\\histogram.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);
        return fullFileName;
    }

    private void AutoContrast(int rightOffset, int leftOffset)
    {
        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i++)
        {
            var ans = Math.Round(Data[i] * 255);
            ans = (ans - leftOffset) / (rightOffset - leftOffset);
            ans = ans < 0 ? 0 : ans;
            ans = ans > 1 ? 1 : ans;
            Data[i] = ans;
        }
    }

    #endregion
}