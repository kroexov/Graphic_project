using System;
using System.Drawing;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public abstract class Pnm
{
    #region Private/protected fields

    protected FileHeaderInfo Header;
    protected int _index;
    protected double[] Data;

    #endregion

    #region Public abstract methods

    public abstract Bitmap CreateBitmap();

    public abstract void ConvertColor(ColorSpace colorSpace);

    public abstract void SetColorChannel(bool[] newColorChannel);

    public abstract void SaveFile(byte[] saveFile);

    #endregion

    #region Private/protected methods

    protected Pnm(byte[] bytes)
    {
        Header = new FileHeaderInfo(ExtractHeaderInfo(bytes));
        
        Data = new double[Header.Width * Header.Height * Header.PixelSize];
        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i++)
        {
            Data[i] = Convert.ToDouble(bytes[i + _index]) / 255.0;
        }
        
        if (Header.Width * Header.Height > bytes.Length - _index)
        {
            throw new Exception("Damaged file");
        }
    }

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
        return y * Header.Width + x;
    }

    #endregion
}