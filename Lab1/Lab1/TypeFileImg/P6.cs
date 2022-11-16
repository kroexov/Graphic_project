using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P6 : PNM
{
    private ColorSpace _colorSpace;
    private bool[] _colorСhannel;
    private Bitmap _img;
    public P6(byte[] bytes) : base(bytes)
    {
        _colorСhannel = new bool[] { false, false, true };
    }

    public void SetColorSpace(ColorSpace colorSpace)
    {
        _colorSpace = colorSpace;
    }

    public void SetColorCanal(int numColorCanal)
    {
        _colorСhannel[numColorCanal] = !_colorСhannel[numColorCanal];
    }

    public override Bitmap CreateBitmap()
    {
        var image = new Bitmap(_header.Width, _header.Height, PixelFormat.Format24bppRgb);

        for (var y = 0; y < _header.Height; y++)
        {
            for (var x = 0; x < _header.Width; x++)
            {
                var valueRed = _data[GetCoordinates(x, y)];
                var valueGreen = _data[GetCoordinates(x + 1, y)];
                var valueBlue = _data[GetCoordinates(x + 2, y)];
                Color newColor = Color.FromArgb((byte)Math.Round(valueRed * Convert.ToInt32(_colorСhannel[0]), 8),
                                                (byte)Math.Round(valueGreen * Convert.ToInt32(_colorСhannel[1]), 8), 
                                                (byte)Math.Round(valueBlue * Convert.ToInt32(_colorСhannel[2]), 8));
                image.SetPixel(x, y, newColor);
            }
            
        }

        _img = image;
        
        return image;
    }
    
    
}