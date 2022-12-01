using System;
using System.Drawing;
using System.Drawing.Imaging;
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
    
    // y1 <= y2
    public void DrawLineWithAntialiasing(int x1, int y1, int x2, int y2, int width = 1, double transparency = 1, Color colorLine = new Color())
    {
        var lineWidth = 3 * (Math.Abs(x1 - x2) + width - (width % 2));
        var lineHeight = 3 * (y2 - y1 + width - (width % 2)); 
        var dataLine = new byte[lineWidth * lineHeight];
        int r;
        if (width % 2 == 0)
            r = (3 * width) / 2;
        else
            r = (3 * width - 1) / 2;
        
        if (x1 < x2)
        {
            var newX1 = r;
            var newY1 = r;
            var newX2 = lineWidth - (r + 1);
            var newY2 = lineHeight - (r + 1);
            DrawCircle(newX1, newY1, newX2, newY2, r, ref dataLine, lineWidth);
        }
        else
        {
            var newX1 = lineWidth - (r - 1);
            var newY1 = r - 1;
            var newX2 = r - 1;
            var newY2 = lineHeight - (r + 1);
            DrawCircle(newX1, newY1, newX2, newY2, r, ref dataLine, lineWidth);
        }

        var image = new Bitmap(103, 103, PixelFormat.Format24bppRgb);
        for (var i = 0; i < lineHeight / 3; i++)
        {
            for (var j = 0; j < lineWidth / 3; j++)
            {
                var value = Convert.ToDouble(dataLine[GetCoordinates(3 * j, 3 * i, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 1, 3 * i, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 2, 3 * i, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j, 3 * i + 1, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 1, 3 * i + 1, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 2, 3 * i + 1, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j, 3 * i + 2, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 1, 3 * i + 2, lineWidth)]
                                                 + dataLine[GetCoordinates(3 * j + 2, 3 * i + 2, lineWidth)]) / 9;
                
                Color newColor = Color.FromArgb((byte)Math.Round(value),
                    (byte)Math.Round(value), 
                    (byte)Math.Round(value));
                
                image.SetPixel(j + 3, i + 3, newColor);
            } 
        }
        
        image.Save("C:\\аниме\\ff.bmp", ImageFormat.Bmp);
    }
    private void DrawLine(int x1, int y1, int x2, int y2, ref byte[] dataLine, int dataWidth)
    {
        var dx = Math.Abs(x1 - x2);
        var dy = y2 - y1;
        var signX = x1 < x2 ? 1 : -1;
        var signY = y1 < y2 ? 1 : -1;
        var error = dx - dy;
        dataLine[GetCoordinates(x2, y2, dataWidth)] = byte.MaxValue;
        
        while (x1 != x2 || y1 != y2)
        {
            dataLine[GetCoordinates(x1, y1, dataWidth)] = byte.MaxValue;
            var error2 = error * 2;
            if (error2 > -dy)
            {
                error = -dy;
                x1 += signX;
            }
            if (error2 < -dx)
            {
                error += dx;
                y1 += signY;
            }
        }
    }

    private void DrawCircle(int x0, int y0, int x1, int y1, int r, ref byte[] data, int dataWidth)
    {
        var dx = x0 - x1;
        var dy = y0 - y1;
        
        var x = 0;
        var y = r;
        var delta = 1 - 2 * r;
        
        DrawLine(x0, y0 - y,  x0 - dx, y0 - y - dy, ref data, dataWidth);
        while(y >= 0) {
            // DrawLine(x0 + x, y0 + y,  x0 + x - dx, y0 + y - dy, ref data, dataWidth);
            // DrawLine(x0 + x, y0 - y,  x0 + x - dx, y0 - y - dy, ref data, dataWidth);
            // DrawLine(x0 - x, y0 + y,  x0 - x - dx, y0 + y - dy, ref data, dataWidth);
            // DrawLine(x0 - x, y0 - y,  x0 - x - dx, y0 - y - dy, ref data, dataWidth);
            
            // data[GetCoordinates(x0 + x, y0 + y, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(x0 + x, y0 - y, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(x0 - x, y0 + y, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(x0 - x, y0 - y, dataWidth)] = byte.MaxValue;
            
            var error = 2 * (delta + y) - 1;
            if(delta < 0 && error <= 0) {
                ++x;
                delta += 2 * x + 1;
                continue;
            }
            error = 2 * (delta - x) - 1;
            if(delta > 0 && error > 0) {
                --y;
                delta += 1 - 2 * y;
                continue;
            }
            ++x;
            delta += 2 * (x - y);
            --y;
        }
        
        var image = new Bitmap(data.Length / dataWidth + 20, dataWidth + 20, PixelFormat.Format24bppRgb);
        for (var i = 0; i < data.Length / dataWidth; i++)
        {
            for (var j = 0; j < dataWidth; j++)
            {
                var value = Convert.ToDouble(data[GetCoordinates(j, i, dataWidth)]);
                
                Color newColor = Color.FromArgb((byte)Math.Round(value),
                    (byte)Math.Round(value), 
                    (byte)Math.Round(value));
                
                image.SetPixel(j + 10, i + 10, newColor);
            } 
        }
        
        image.Save("C:\\аниме\\test.bmp", ImageFormat.Bmp);
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

    protected int GetCoordinates(int x, int y, int width)
    {
        return y * width + x;
    }

    #endregion
}