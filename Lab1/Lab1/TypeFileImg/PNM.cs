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
    
    public void ConvertGamma(double newConvertGamma)
    {
        var oldConvertGamma = _gammaConvert;
        _gammaConvert = newConvertGamma;
        
        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i++)
        {
            Data[i] = ConvertFromOldGammaToNewGamma(Data[i], oldConvertGamma);
        }
    }

    #endregion

    #region Private/protected methods

    private double ConvertFromSrgb(double srgbValue)
    {
        if (srgbValue <= _valueConvertSrgbToRgb[0])
        {
            return srgbValue / _valueConvertSrgbToRgb[1];
        }
        return Math.Pow((srgbValue + _valueConvertSrgbToRgb[2]) / _valueConvertSrgbToRgb[3], _valueConvertSrgbToRgb[4]);
    }

    private double ConvertToSrgb(double rgbValue)
    {
        if (rgbValue <= _valueConvertSrgbToRgb[0] / _valueConvertSrgbToRgb[1])
        {
            return _valueConvertSrgbToRgb[1] * rgbValue;
        }

        return Math.Pow(rgbValue, 1 / _valueConvertSrgbToRgb[4]) * _valueConvertSrgbToRgb[3] -
               _valueConvertSrgbToRgb[2];
    }

    protected double AssignGamma(double rgbValue)
    {
        if (_gammaAssign == 0 && _gammaConvert == 0)
        {
            return rgbValue;
        }
        else if (_gammaAssign == 0 && _gammaConvert != 0)
        {
            return ConvertToSrgb(Math.Pow(rgbValue, _gammaConvert));
        }
        else if (_gammaAssign != 0 && _gammaConvert == 0)
        {
            return Math.Pow(ConvertFromSrgb(rgbValue), 1 / _gammaAssign);
        }
        else
        {
            return Math.Pow(Math.Pow(rgbValue, _gammaConvert), 1 / _gammaAssign);
        }
    }

    private double ConvertFromOldGammaToNewGamma(double value, double oldConvertGamma)
    {
        if (oldConvertGamma == 0)
        {
            value = ConvertFromSrgb(value);
        }
        else
        {
            value = Math.Pow(value, oldConvertGamma);
        }

        if (_gammaConvert == 0)
        {
            value = ConvertToSrgb(value);
        }
        else
        {
            value = Math.Pow(value, 1 / _gammaConvert);
        }

        return value;
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
            Data[i] = Convert.ToDouble(bytes[i + _index]) / 255.0;
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