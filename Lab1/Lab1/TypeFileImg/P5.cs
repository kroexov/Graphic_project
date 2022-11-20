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
        var image = new Bitmap(Header.Width, Header.Height, PixelFormat.Format8bppIndexed);

        for (var x = 0; x < Header.Width; x++)
        {
            for (var y = 0; y < Header.Height; y++)
            {
                var valueColor = 255 * Data[GetCoordinates(x, y)];
                Color newColor = Color.FromArgb((byte)Math.Round(valueColor, 8), (byte)Math.Round(valueColor, 8), (byte)Math.Round(valueColor, 8));
                image.SetPixel(x, y, newColor);
            }
        }
        
        ColorPalette palette = image.Palette;
        var entries = palette.Entries;
        for (var i = 0; i < 256; i++)
        {
            Color b = Color.FromArgb((byte)i, (byte)i, (byte)i);
            entries[i] = b;
        }
        image.Palette = palette;

        return image;
    }

    public override void ConvertColor(ColorSpace colorSpace)
    {
        throw new NotImplementedException();
    }

    public override void SetColorChannel(bool[] newColorChannel)
    {
        throw new NotImplementedException();
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

    #endregion
}