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
    protected double[] _valueConvertSrgbToRgb = {0.04045, 12.92, 0.055, 1.055, 2.4};
    protected double _gammaAssign = 0;
    protected double _gammaConvert = 0;

    #endregion

    #region Public abstract methods

    public abstract Bitmap CreateBitmap();

    public abstract void ConvertColor(ColorSpace colorSpace);

    public abstract void SetColorChannel(bool[] newColorChannel);

    public abstract byte[] SaveFile(byte[] origFile);

    public void SetGammaСoefficient(double newGamma)
    {
        _gammaAssign = newGamma;
    }

    public void ConvertGammaImage(double gammaConvertValue)
    {
        _gammaConvert = gammaConvertValue;

        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i++)
        {
            Data[i] = ConvertColorModelToRgb(ConvertRgbToColorModel(Data[i]));
        }

        _gammaAssign = gammaConvertValue;
    }

    #endregion

    #region Private/protected methods
    
    protected double ConvertColorModelToRgb(double linearValue)
    {
        if (_gammaConvert == 0)
        {
            if (linearValue <= _valueConvertSrgbToRgb[0])
            {
                return linearValue / _valueConvertSrgbToRgb[1];
            }
            return Math.Pow((linearValue + _valueConvertSrgbToRgb[2]) / _valueConvertSrgbToRgb[3], _valueConvertSrgbToRgb[4]);
        }

        return Math.Pow(linearValue, _gammaConvert);
    }

    protected double ConvertRgbToColorModel(double rgbValue)
    {
        if (_gammaAssign == 0)
        {
            if (rgbValue <= _valueConvertSrgbToRgb[0] / _valueConvertSrgbToRgb[1])
            {
                return _valueConvertSrgbToRgb[1] * rgbValue;
            }

            return Math.Pow(rgbValue, 1 / _valueConvertSrgbToRgb[4]) * _valueConvertSrgbToRgb[3] -
                   _valueConvertSrgbToRgb[2];
        }

        return Math.Pow(rgbValue, 1 / _gammaAssign);
    }

    protected Pnm(byte[] bytes)
    {
        Header = new FileHeaderInfo(ExtractHeaderInfo(bytes));
        
        if (Header.Width * Header.Height * Header.PixelSize > bytes.Length - _index)
        {
            throw new Exception("Damaged file");
        }
        
        Data = new double[Header.Width * Header.Height * Header.PixelSize];

        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i++)
        {
            Data[i] = ConvertColorModelToRgb(Convert.ToDouble(bytes[i + _index]) / 255.0);
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