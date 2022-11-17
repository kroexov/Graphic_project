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

    #endregion
}