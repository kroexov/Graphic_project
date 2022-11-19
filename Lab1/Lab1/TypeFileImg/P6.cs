using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P6 : Pnm
{
    #region Private fields

    private ColorSpace _colorSpace;
    private bool[] _colorСhannel;
    private Bitmap _img;

    #endregion

    #region Constructor

    public P6(byte[] bytes) : base(bytes)
    {
        _colorСhannel = new bool[] { true, true, true };
        _colorSpace = ColorSpace.Rgb;
    }

    #endregion

    #region Public methods
    
    public void SetColorCanal(int numColorCanal)
    {
        _colorСhannel[numColorCanal] = !_colorСhannel[numColorCanal];
    }

    public override Bitmap CreateBitmap()
    {
        var image = new Bitmap(Header.Width, Header.Height, PixelFormat.Format24bppRgb);

        for (var y = 0; y < Header.Height; y++)
        {
            for (var x = 0; x < Header.Width; x++)
            {
                var value1 = Data[GetCoordinates(3*x, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                var value2 = Data[GetCoordinates(3*x + 1, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                var value3 = Data[GetCoordinates(3*x + 2, 3*y)]  * Convert.ToInt32(_colorСhannel[0]);
                
                var rgbPixel = ConvertColorPixel(value1, value2, value3, ColorSpace.Rgb);
                
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
        if (_colorSpace == colorSpace)
        {
            return;
        }
        
        if (_colorSpace != ColorSpace.Rgb && colorSpace != ColorSpace.Rgb)
        {
            ConvertColor(ColorSpace.Rgb);
        }
        for (var y = 0; y < Header.Height; y++)
        {
            for (var x = 0; x < Header.Width; x++)
            {
                // значение цвета от 0 до 1;
                var value1 = Data[GetCoordinates(3*x, 3*y)];
                var value2 = Data[GetCoordinates(3*x + 1, 3*y)];
                var value3 = Data[GetCoordinates(3*x + 2, 3*y)];
                
                var newPixel = ConvertColorPixel(value1, value2, value3, colorSpace);
                
                Data[GetCoordinates(3*x, 3*y)] = newPixel[0];
                Data[GetCoordinates(3*x + 1, 3*y)] = newPixel[1];
                Data[GetCoordinates(3*x + 2, 3*y)] = newPixel[2];
            }
        }
        SetColorSpace(colorSpace);
    }

    #endregion

    #region Private methods

    private double[] ConvertColorPixel(double value1, double value2, double value3, ColorSpace colorSpace)
    {
        double[] pixel = new double[3];

        if (_colorSpace == ColorSpace.Rgb && colorSpace == ColorSpace.Rgb)
        {
            return new[] {value1, value2, value3};
        }

        if (_colorSpace == ColorSpace.Rgb)
        {
            switch (colorSpace)
            {
                case ColorSpace.Hsl:
                {
                    pixel = RgbToHsl(value1, value2, value3);
                    break;
                }
                case ColorSpace.Hsv:
                {
                    RgbToHsv(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr601:
                {
                    pixel = RgbToYСbСr601(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr709:
                {
                    pixel = RgbToYСbСr709(value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    pixel = RgbToYCoCg(value1, value2, value3);
                    break;
                }
                case ColorSpace.Cmy:
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
                case ColorSpace.Hsl:
                {
                    pixel = HslToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.Hsv:
                {
                    pixel = HsvToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr601:
                {
                    pixel = YСbСr601ToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr709:
                {
                    pixel = YСbСr709ToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    pixel = YCoCgToRgb(value1, value2, value3);
                    break;
                }
                case ColorSpace.Cmy:
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
        double maxv = Math.Max(green, Math.Max(red, blue));
        double minv = Math.Min(green, Math.Min(red, blue));
        pixel[2] = (maxv + minv) / 2;
        pixel[1] = CountS(pixel[2], maxv, minv);
        pixel[0] = CountH(red, green, blue, minv, maxv);
        //конец
        
        return pixel;
    }

    private double CountS(double L, double maxv, double minv)
    {
        if (L==0 || Equals(maxv, minv))
        {
            return 0;
        }

        return (L > 0 && L <= 0.5) ? (maxv - minv) / (2 * L) : (maxv - minv) / (2 - 2 * L);
    }

    private double CountH(double r, double g, double b, double minv, double maxv)
    {
        if (Equals(maxv, minv) || (Equals(maxv, r) && g >= b))
        {
            return 1.0 / 6 * (g - b) / (maxv - minv);
        }

        if (maxv.Equals(r) && g < b)
        {
            return 1.0 / 6 * (g - b) / (maxv - minv) + 1;
        }

        if (maxv.Equals(g))
        {
            return 1.0 / 6 * (b - r) / (maxv - minv) + 1.0 / 3;
        }

        if (maxv.Equals(b))
        {
            return 1.0 / 6 * (r - g) / (maxv - minv) + 2.0 / 3;
        }

        return 0;
    }

    private double[] HslToRgb(double h, double s, double l)
    {
        var pixel = new double[3];
        
        //начало конвертации
        //пока считаем что H хранится от 0 до 1
        double q;
        if (l < 0.5)
        {
            q = l * (1 + s);
        }
        else
        {
            q = l + s - (l * s);
        }

        double p = 2.0 * l - q;
        
        // double h_k = h / 360;
        double h_k = h;
        
        double t_r = h_k + 1.0 / 3;
        t_r -= ((t_r > 1) ? 1 : 0);
        
        double t_g = h_k;
        
        double t_b = h_k - 1.0 / 3;
        t_b += ((t_b < 0) ? 1 : 0);

        pixel[0] = CalculateColor(p, q, t_r);
        pixel[1] = CalculateColor(p, q, t_g);
        pixel[2] = CalculateColor(p, q, t_b);
        //конец
        
        return pixel;
    }

    private double CalculateColor(double p, double q, double t)
    {
        if (t < 1.0 / 6)
        {
            return p + ((q - p) * 6 * t);
        }

        if (t >= 1.0 / 6 && t < 1.0 / 2)
        {
            return q;
        }

        if (t >= 1.0 / 2 && t < 2.0 / 3)
        {
            return p + ((q - p) * (2.0 / 3 - t) * 6);
        }

        return p;
    }

    private void RgbToHsv(double[] pixel, double red, double green, double blue)
    {
        double maxv = Math.Max(green, Math.Max(red, blue));
        double minv = Math.Min(green, Math.Min(red, blue));
        pixel[2] = maxv;
        pixel[1] = (maxv == 0) ? 0 : 1.0 - minv / maxv;
        pixel[0] = countHHsv(red, green, blue, maxv, minv);
    }

    private double countHHsv(double r, double g, double b, double maxv, double minv)
    {
        if (maxv.Equals(r) && g >= b)
        {
            return 1.0 / 6 * (g - b) / (maxv - minv);
        }
        if (maxv.Equals(r) && g < b)
        {
            return 1.0 / 6 * (g - b) / (maxv - minv) + 1.0;
        }
        if (maxv.Equals(g))
        {
            return 1.0 / 6 * (b - r) / (maxv - minv) + 1.0/3;
        }
        if (maxv.Equals(b))
        {
            return 1.0 / 6 * (r - g) / (maxv - minv) + 2.0/3;
        }

        return 0;
    }

    private double[] HsvToRgb(double h, double s, double v)
    {
        var pixel = new double[3];
        
        //начало конвертации
        int H = (int)(h * 360);
        int h_i = (H / 60) % 6;

        double v_min = (1 - s) * v;
        double a = (v - v_min) * (((H % 60) / 60.0));
        double v_inc = v_min + a;
        double v_dec = v - a;

        switch (h_i)
        {
            case 0:
                pixel[0] = v;
                pixel[1] = v_inc;
                pixel[2] = v_min;
                break;
            case 1:
                pixel[0] = v_dec;
                pixel[1] = v;
                pixel[2] = v_min;
                break;
            case 2:
                pixel[0] = v_min;
                pixel[1] = v;
                pixel[2] = v_inc;
                break;
            case 3:
                pixel[0] = v_min;
                pixel[1] = v_dec;
                pixel[2] = v;
                break;
            case 4:
                pixel[0] = v_inc;
                pixel[1] = v_min;
                pixel[2] = v;
                break;
            case 5:
                pixel[0] = v;
                pixel[1] = v_min;
                pixel[2] = v_dec;
                break;
        }
        //конец
        
        return pixel;
    }

    private double[] RgbToYСbСr601(double red, double green, double blue)
    {
        var pixel = new double[3];

        pixel[0] = (16 + (65.481 * red + 128.553 * green + 24.966 * blue)) / 256;
        pixel[1] = (128 + (-37.797 * red - 74.203 * green + 112.0 * blue)) / 256;
        pixel[2] = (128 + (112.0 * red - 93.786 * green - 18.214 * blue)) / 256;
        return pixel;
    }

    private double[] YСbСr601ToRgb(double y, double Cb, double Cr)
    {
        var pixel = new double[3];

        pixel[0] = (255.0 / 219) * (y * 256 - 16) + (255.0 / 112) * 0.701 * (Cr * 256 - 128);
        pixel[0] /= 255;
        pixel[1] = (255.0 / 219) * (y * 256 - 16) - (255.0 / 112) * 0.886 * 0.114 / 0.587 * (Cb * 256 - 128) -
                   (255.0 / 112) * 0.701 * 0.299 / 0.587 * (Cr * 256 - 128);
        pixel[1] /= 255;
        pixel[2] = (255.0 / 219) * (y * 256 - 16) + (255.0 / 112) * 0.886 * (Cb * 256 - 128);
        pixel[2] /= 255;
        
        return pixel;
    }

    private double[] RgbToYСbСr709(double red, double green, double blue)
    {
        var pixel = new double[3];

        pixel[0] = (0.299 * red*255) + (0.587 * green*255) + (0.114 * blue*255);
        pixel[0] /= 255;
        pixel[1] = 128 - (0.168736 * red*255) - (0.331264 * green*255) + (0.5 * blue*255);
        pixel[1] /= 255;
        pixel[2] = 128 + (0.5 * red*255) - (0.418688 * green*255) - (0.081312 * blue*255);
        pixel[2] /= 255;
        return pixel;
    }

    private double[] YСbСr709ToRgb(double y, double Cb, double Cr)
    {
        var pixel = new double[3];

        pixel[0] = y + 1.402 * (Cr * 255 - 128);
        pixel[0] /= 255;
        pixel[1] = y - 0.34414 * (Cb * 255 - 128) - 0.71414 * (Cr * 255 - 128);
        pixel[1] /= 255;
        pixel[2] = y + 1.772 * (Cb * 255 - 128);
        pixel[2] /= 255;
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
    
    private void SetColorSpace(ColorSpace colorSpace)
    {
        _colorSpace = colorSpace;
    }

    #endregion
}