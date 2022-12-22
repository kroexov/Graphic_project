using System;
using System.Drawing;
using System.Drawing.Imaging;
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

    public override void Scale(string scalingAlgorithm, int height, int width)
    {
        switch (scalingAlgorithm)
        {
            case "Closest point":
                ClosestPointScale(height, width);
                break;
            case "Bilinear":
                break;
            case "Lanczos3":
                break;
            case "BC-splines":
                break;
        }
    }

    #endregion
    
    private void ClosestPointScale(int newHeight, int newWidth)
    {
        var newData = new double[Header.PixelSize * newHeight * newWidth];

        for (var y = 0; y < newHeight; y++)
        {
            var oldY = (int)Math.Ceiling(y * (double)Header.Height / newHeight);
            if (oldY == Header.Height)
                oldY--;

            for (var x = 0; x < newWidth; x++)
            {
                var oldX = (int)Math.Ceiling(x * (double)Header.Width / newWidth);

                if (oldX == Header.Width)
                    oldX--;
                
                var value1 = Data[GetCoordinates(oldX, oldY)];

                newData[y * newWidth + x] = value1;
            }
        }

        Data = newData;
        Header.Width = newWidth;
        Header.Height = newHeight;
    }
}