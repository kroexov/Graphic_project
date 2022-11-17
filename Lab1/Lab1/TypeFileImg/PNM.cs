using System;
using System.Drawing;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public abstract class PNM
{
    protected FileHeaderInfo _header;
    private int _index;
    protected double[] _data;
    
    protected PNM(byte[] bytes)
    {
        _header = new FileHeaderInfo(ExtractHeaderInfo(bytes));
        
        _data = new double[_header.Width * _header.Height * _header.PixelSize];
        for (var i = 0; i < _header.Width * _header.Height * _header.PixelSize; i++)
        {
            _data[i] = Convert.ToDouble(bytes[i + _index]) / 255.0;
        }
        
        if (_header.Width * _header.Height > bytes.Length - _index)
        {
            throw new Exception("Damaged file");
        }
    }

    public abstract Bitmap CreateBitmap();
    
    private string ExtractHeaderInfo(byte[] bytes)
    {
        var header = "";
        var lineBreakCounter = 0;
        const int codeOfLineBreakChar = 10;
        _index = 0;

        while (lineBreakCounter != 3)
        {
            if (bytes[_index] == codeOfLineBreakChar)
            {
                lineBreakCounter++;
                header += " ";
            }
            else
            {
                header += Convert.ToChar(bytes[_index]);
            }

            _index++;
        }

        return header;
    }

    protected int GetCoordinates(int x, int y)
    {
        return _header.PixelSize * y * _header.Width + _header.PixelSize * x;
    }
}