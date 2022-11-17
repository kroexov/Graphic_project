using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P5 : PNM
{
    
    public P5(byte[] bytes) : base(bytes)
    {
        
    }

    public override Bitmap CreateBitmap()
    {
        var image = new Bitmap(_header.Width, _header.Height, PixelFormat.Format8bppIndexed);

        for (var x = 0; x < _header.Width; x++)
        {
            for (var y = 0; y < _header.Height; y++)
            {
                var valueColor = 255 * _data[GetCoordinates(x, y)];
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
}