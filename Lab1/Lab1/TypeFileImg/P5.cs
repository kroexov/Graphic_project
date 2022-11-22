using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P5 : Pnm
{
    private double gamma = 2.2;

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
                var valueColor = 255 * ConvertRgbToColorModel(Data[GetCoordinates(x, y)]);
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
            saveFile[i + _index] = (byte)Math.Round(ConvertRgbToColorModel(Data[i]) * 255);

        
        return saveFile;
    }

    #endregion
}