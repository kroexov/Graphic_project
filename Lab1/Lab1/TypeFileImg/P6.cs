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
        _colorСhannel = new bool[] { true, true, true };
        _colorSpace = ColorSpace.RGB;
    }

    private void SetColorSpace(ColorSpace colorSpace)
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
                var value1 = _data[GetCoordinates(3*x, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                var value2 = _data[GetCoordinates(3*x + 1, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                var value3 = _data[GetCoordinates(3*x + 2, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                
                var rgbPixel = ConvertColorPixel(value1, value2, value3, ColorSpace.RGB);
                
                var valueRed = 255 * rgbPixel[0];
                var valueGreen = 255 * rgbPixel[1];
                var valueBlue = 255 * rgbPixel[2];
                
                Color newColor = Color.FromArgb((byte)Math.Round(valueRed),
                                                (byte)Math.Round(valueGreen), 
                                                (byte)Math.Round(valueBlue));
                
                image.SetPixel(x, y, newColor);
            }
            
        }

        _img = image;
        
        return image;
    }

    public override void ConvertColor(ColorSpace colorSpace)
    {
        if (_colorSpace == ColorSpace.RGB && colorSpace == ColorSpace.RGB)
        {
            return;
        }
        
        if (_colorSpace != ColorSpace.RGB && colorSpace != ColorSpace.RGB)
        {
            ConvertColor(ColorSpace.RGB);
        }
        for (var y = 0; y < _header.Height; y++)
        {
            for (var x = 0; x < _header.Width; x++)
            {
                // значение цвета от 0 до 1;
                var value1 = _data[GetCoordinates(3*x, 3*y)];
                var value2 = _data[GetCoordinates(3*x + 1, 3*y)];
                var value3 = _data[GetCoordinates(3*x + 2, 3*y)];
                
                var newPixel = ConvertColorPixel(value1, value2, value3, colorSpace);
                
                _data[GetCoordinates(3*x, 3*y)] = newPixel[0];
                _data[GetCoordinates(3*x + 1, 3*y)] = newPixel[1];
                _data[GetCoordinates(3*x + 2, 3*y)] = newPixel[2];
            }
        }
        SetColorSpace(colorSpace);
    }
    
    private double[] ConvertColorPixel(double value1, double value2, double value3, ColorSpace colorSpace)
    {
        double[] pixel;

        if (_colorSpace == ColorSpace.RGB && colorSpace == ColorSpace.RGB)
        {
            return new[] {value1, value2, value3};
        }

        if (_colorSpace == ColorSpace.RGB)
        {
            switch (colorSpace)
            {
                case ColorSpace.HSL:
                {
                    pixel = RgbToHsl(value1, value2, value3);
                    break;
                }
                case ColorSpace.HSV:
                {
                    pixel = RgbToHsv(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCR601:
                {
                    pixel = RgbToYСbСr601(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCR709:
                {
                    pixel = RgbToYСbСr709(value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    pixel = RgbToYCoCg(value1, value2, value3);
                    break;
                }
                case ColorSpace.CMY:
                {
                    pixel = RgbToCmy(value1, value2, value3);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace), colorSpace, null);
            }
        }
        else
        {
            switch (_colorSpace)
            {
                case ColorSpace.HSL:
                {
                    pixel = HslToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.HSV:
                {
                    pixel = HsvToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCR601:
                {
                    pixel = YСbСr601ToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCR709:
                {
                    pixel = YСbСr709ToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    pixel = YCoCgToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.CMY:
                {
                    pixel = CmyToRgb(value1, value2, value3);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace), colorSpace, null);
            }
        }
        
        return pixel;
    }

    private double[] RgbToCmy(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        pixel[0] = 1 - red;
        pixel[1] = 1 - green;
        pixel[2] = 1 - blue;
        //конец
        
        return pixel;
    }

    private double[] CmyToRgb(double cyan, double magenta, double yellow)
    {
        var pixel = new double[3];
        
        //начало конвертации
        pixel[0] = 1 - cyan;
        pixel[1] = 1 - magenta;
        pixel[2] = 1 - yellow;
        //конец
        
        return pixel;
    }

    private double[] RgbToHsl(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] HslToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] RgbToHsv(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] HsvToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] RgbToYСbСr601(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] YСbСr601ToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] RgbToYСbСr709(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] YСbСr709ToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] RgbToYCoCg(double red, double green, double blue)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }

    private double[] YCoCgToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //конец
        
        return pixel;
    }
}