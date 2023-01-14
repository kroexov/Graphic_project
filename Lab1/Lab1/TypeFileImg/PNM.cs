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
    public void DrawLineWithAntialiasing(int x1, int y1, int x2, int y2, int width = 3, double transparency = 1,
        double color1 = 1, double color2 = 1, double color3 = 1)
    {
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        var isSwap = false;
        
        if (y1 > y2)
        {
            Swap(ref y1, ref y2);
            Swap(ref x1, ref x2);
            isSwap = true;
        }
        var lineWidth = 3 * (Math.Abs(x1 - x2) + width - (width % 2));
        var lineHeight = 3 * (y2 - y1 + width - (width % 2)); 
        var dataLine = new byte[lineWidth * lineHeight];
        int r;
        if (width % 2 == 0)
            r = (3 * width) / 2;
        else
            r = (3 * width - 1) / 2;
        int newX1, newY1, newX2, newY2;
        if (x1 < x2)
        {
            newX1 = r + 1 - width % 2;
            newY1 = r + 1 - width % 2;
            newX2 = lineWidth - (r + 1 + 1 - width % 2);
            newY2 = lineHeight - (r + 1 + 1 - width % 2);
            DrawCircle(newX1, newY1, newX2, newY2, r, ref dataLine, lineWidth);
        }
        else
        {
            newX1 = lineWidth - (r + 1 + 1 - width % 2);
            newY1 = r + 1 - width % 2;
            newX2 = r + 1 - width % 2;
            newY2 = lineHeight - (r + 1 + 1 - width % 2);
            DrawCircle(newX1, newY1, newX2, newY2, r, ref dataLine, lineWidth);
        }

        newX1 = newX1 / 3;
        newY1 = newY1 / 3;

        for (var y = 1; y < lineHeight - 1; y++)
        {
            for (var x = 1; x < lineWidth - 1; x++)
            {
                if (dataLine[GetCoordinates(x - 1, y, lineWidth)] == byte.MaxValue &&
                    dataLine[GetCoordinates(x, y - 1, lineWidth)] == byte.MaxValue &&
                    dataLine[GetCoordinates(x + 1, y, lineWidth)] == byte.MaxValue &&
                    dataLine[GetCoordinates(x, y + 1, lineWidth)] == byte.MaxValue)
                {
                    dataLine[GetCoordinates(x, y, lineWidth)] = byte.MaxValue;
                }
            }
        }

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
                                                 + dataLine[GetCoordinates(3 * j + 2, 3 * i + 2, lineWidth)]) / 9 /255;

                value = ConvertFromOldGammaToNewGamma(value, 1);
                
                if (value == 0 || (x1 < newX1) || y1 < newY1)
                {
                    continue;
                }
                if (Header.FileFormat == "P6")
                {
                    Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1))] =
                        (1-transparency) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1))] + transparency * 
                        ((1 - value) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1))] + value * color1);
                    Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 1] =
                        (1-transparency) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 1] + transparency * 
                        ((1 - value) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 1] + value * color2);
                    Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 2] =
                        (1-transparency) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 2] + transparency * 
                        ((1 - value) * Data[GetCoordinates(3 * (j + x1 - newX1), 3 * (i + y1 - newY1)) + 2] + value * color3);
                }
                else if (Header.FileFormat == "P5")
                {
                    Data[GetCoordinates(j + x1 - newX1, i + y1 - newY1)] =
                        (1-transparency) * Data[GetCoordinates(j + x1 - newX1, i + y1 - newY1)] + transparency * 
                        ((1 - value) * Data[GetCoordinates(j + x1 - newX1, i + y1 - newY1)] + value * color1);
                }
            } 
        }
    }
    private void DrawLine(int x0, int y0, int x1, int y1, ref byte[] dataLine, int dataWidth)
    {
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        
        var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Проверяем рост отрезка по оси икс и по оси игрек
        // Отражаем линию по диагонали, если угол наклона слишком большой
        if (steep)
        {
            Swap(ref x0, ref y0); // Перетасовка координат вынесена в отдельную функцию для красоты
            Swap(ref x1, ref y1);
        }
        // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }
        var dx = x1 - x0;
        var dy = Math.Abs(y1 - y0);
        var error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
        var yStep = (y0 < y1) ? 1 : -1; // Выбираем направление роста координаты y
        var y = y0;
        for (var x = x0; x <= x1; x++)
        {
            dataLine[GetCoordinates(steep ? y : x, steep ? x : y, dataWidth)] = byte.MaxValue; // Не забываем вернуть координаты на место
            error -= dy;
            if (error < 0)
            {
                y += yStep;
                error += dx;
            }
        }
    }

    private void DrawCircle(int x0, int y0, int x1, int y1, int r, ref byte[] data, int dataWidth)
    {
        var dx = x1 - x0;
        var dy = y1 - y0;

        int x = r;
        int y = 0;
        int radiusError = 1 - x;
        while (x >= y)
        {
            DrawLine(x + x0, y + y0, x + x0 + dx, y + y0 + dy, ref data, dataWidth);
            DrawLine(y + x0, x + y0, y + x0 + dx, x + y0 + dy, ref data, dataWidth);
            DrawLine(-x + x0, y + y0, -x + x0 + dx, y + y0 + dy, ref data, dataWidth);
            DrawLine(-y + x0, x + y0, -y + x0 + dx, x + y0 + dy, ref data, dataWidth);
            DrawLine(-x + x0, -y + y0, -x + x0 + dx, -y + y0 + dy, ref data, dataWidth);
            DrawLine(-y + x0, -x + y0, -y + x0 + dx, -x + y0 + dy, ref data, dataWidth);
            DrawLine(x + x0, -y + y0, x + x0 + dx, -y + y0 + dy, ref data, dataWidth);
            DrawLine(y + x0, -x + y0, y + x0 + dx, -x + y0 + dy, ref data, dataWidth);
            
            DrawLine(-x + 1 + x0, -y + y0, y + x0 + dx, x + y0 + dy - 1, ref data, dataWidth);
            DrawLine(-y + x0, -x + y0 + 1, x + x0 + dx - 1, y + y0 + dy, ref data, dataWidth);
            
            DrawLine(x - 1 + x0, y + y0, -y + x0 + dx, -x + y0 + dy + 1, ref data, dataWidth);
            DrawLine(y + x0, x + y0 - 1, -x + x0 + dx + 1, -y + y0 + dy, ref data, dataWidth);

            // data[GetCoordinates(x + x0, y + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(y + x0, x + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(-x + x0, y + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(-y + x0, x + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(-x + x0, -y + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(-y + x0, -x + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(x + x0, -y + y0, dataWidth)] = byte.MaxValue;
            // data[GetCoordinates(y + x0, -x + y0, dataWidth)] = byte.MaxValue;
            y++;
            if (radiusError < 0)
            {
                radiusError += 2 * y + 1;
            }
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
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

    protected int GetCoordinates(int x, int y, int width)
    {
        return y * width + x;
    }

    #endregion
}