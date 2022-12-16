using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Lab1.Models;

namespace Lab1.TypeFileImg;

public class P6 : Pnm
{
    #region Private fields

    private bool[] _currentColorСhannel;
    private ColorSpace _currentColorSpace;
    private Bitmap _img;
    private double[] tempPixel = new double[3];

    #endregion

    #region Constructor

    public P6(byte[] bytes) : base(bytes)
    {
        _currentColorСhannel = new bool[] { true, true, true };
        _currentColorSpace = ColorSpace.Rgb;
    }

    public P6(byte[] bytes, ColorSpace colorSpace) : base(bytes)
    {
        _currentColorСhannel = new bool[] { true, true, true };
        _currentColorSpace = colorSpace;
    }

    #endregion

    #region Public methods

    public override Bitmap CreateBitmap()
    {
        var image = new Bitmap(Header.Width, Header.Height, PixelFormat.Format24bppRgb);

        for (var y = 0; y < Header.Height; y++)
        {
            for (var x = 0; x < Header.Width; x++)
            {
                var value1 = Data[GetCoordinates(3*x, 3*y)]  * Convert.ToInt32(_currentColorСhannel[0]);
                var value2 = Data[GetCoordinates(3*x + 1, 3*y)]  * Convert.ToInt32(_currentColorСhannel[1]);
                var value3 = Data[GetCoordinates(3*x + 2, 3*y)]  * Convert.ToInt32(_currentColorСhannel[2]);
                
                ConvertColorPixel(tempPixel, value1, value2, value3, ColorSpace.Rgb);
                
                var valueRed = 255 * tempPixel[0];
                var valueGreen = 255 * tempPixel[1];
                var valueBlue = 255 * tempPixel[2];
                
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
        if (_currentColorSpace == colorSpace)
        {
            return;
        }
        
        if (_currentColorSpace != ColorSpace.Rgb && colorSpace != ColorSpace.Rgb)
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
                ConvertColorPixel(tempPixel, value1, value2, value3, colorSpace);
                
                Data[GetCoordinates(3*x, 3*y)] = tempPixel[0];
                Data[GetCoordinates(3*x + 1, 3*y)] = tempPixel[1];
                Data[GetCoordinates(3*x + 2, 3*y)] = tempPixel[2];
            }
        }
        SetColorSpace(colorSpace);
    }

    public override void SetColorChannel(bool[] newColorChannel)
    {
        _currentColorСhannel = newColorChannel;
    }

    public override byte[] SaveFile(byte[] origFile)
    {
        var c = -1;
        if (_currentColorСhannel[0] && !_currentColorСhannel[1] && !_currentColorСhannel[2])
            c = 0;
        else if (!_currentColorСhannel[0] && _currentColorСhannel[1] && !_currentColorСhannel[2])
            c = 1;
        else if (!_currentColorСhannel[0] && !_currentColorСhannel[1] && _currentColorСhannel[2])
            c = 2;

        byte[]? saveFile;
        if (c != -1)
        {
            saveFile = new byte[Header.Height * Header.Width + _index];

            for (var i = 0; i < _index; i++)
                saveFile[i] = origFile[i];

            if (saveFile[1] == Convert.ToByte(54))
            {
                saveFile[1] = Convert.ToByte(53);
            }

            for (var i = 0; i < Header.Height * Header.Width; i++)
                saveFile[i + _index] = (byte)Math.Round(Data[i*3 + c] * 255);

            return saveFile;
        }
        
        saveFile = new byte[Header.Height * Header.Width * Header.PixelSize + _index];

        for (var i = 0; i < _index; i++)
            saveFile[i] = origFile[i];
        
        for (var i = 0; i < Header.Height * Header.Width * Header.PixelSize; i++)
            saveFile[i + _index] = (byte)Math.Round(Data[i] * 255 * Convert.ToInt32(_currentColorСhannel[i % 3]));

        return saveFile;
    }

    public override string CreateColorHistogram(double valueIgnore)
    {
        var histogramFirstChannel = new int[256];
        var histogramSecondChannel = new int[256];
        var histogramThirdChannel = new int[256];

        for (var i = 0; i < Header.PixelSize * Header.Height * Header.Width; i += 3)
        {
            // это шаманство из-за NaN в алгоритмах HSL/HSV
            var firstChannel = 0;
            var secondChannel = 0;
            var thirdChannel = 0;
            if (!Data[i].Equals(Double.NaN) & Data[i] > 0)
                firstChannel = Convert.ToInt32(Math.Round(Data[i] * 255));
            if (!Data[i + 1].Equals(Double.NaN) & Data[i + 1] > 0)
                secondChannel = Convert.ToInt32(Math.Round(Data[i + 1] * 255));
            if (!Data[i + 2].Equals(Double.NaN) & Data[i + 2] > 0)
                thirdChannel = Convert.ToInt32(Math.Round(Data[i + 2] * 255));
            histogramFirstChannel[firstChannel] += _currentColorСhannel[0] ? 1 : 0;
            histogramSecondChannel[secondChannel] += _currentColorСhannel[1] ? 1 : 0;
            histogramThirdChannel[thirdChannel] += _currentColorСhannel[2] ? 1 : 0;
        }

        var maxValue = histogramFirstChannel.Max();
        if (maxValue < histogramSecondChannel.Max())
            maxValue = histogramSecondChannel.Max();
        if (maxValue < histogramThirdChannel.Max())
            maxValue = histogramThirdChannel.Max();
        
        var image = new Bitmap(256*3, 768, PixelFormat.Format24bppRgb);
        for (var x = 0; x < 256; x++)
        {
            for (var y = Convert.ToInt32(Math.Round(768.0/maxValue * (maxValue - histogramFirstChannel[x]))); y < 768; y++)
            {
                image.SetPixel(3*x, y, Color.Red);
            }
            for (var y = Convert.ToInt32(Math.Round(768.0/maxValue * (maxValue - histogramSecondChannel[x]))); y < 768; y++)
            {
                image.SetPixel(3*x + 1, y, Color.Lime);
            }
            for (var y = Convert.ToInt32(Math.Round(768.0/maxValue * (maxValue - histogramThirdChannel[x]))); y < 768; y++)
            {
                image.SetPixel(3*x + 2, y, Color.Blue);
            }
        }
        var ignoreValuePixel = 0;
        if (valueIgnore != 0)
        {
            ignoreValuePixel = Convert.ToInt32(Math.Round(256 * valueIgnore)) - 1;
        }
        

        var leftOffset = ignoreValuePixel;
        for (var i = ignoreValuePixel; i < 256 && histogramFirstChannel[i] == 0
                                                  && histogramSecondChannel[i] == 0
                                                  && histogramThirdChannel[i] == 0; i++)
        {
            leftOffset = i;
        }

        var rightOffset = 255 - ignoreValuePixel;
        for (var i = 255 - ignoreValuePixel; i >= 0 && histogramFirstChannel[i] == 0
                                                    && histogramSecondChannel[i] == 0
                                                    && histogramThirdChannel[i] == 0; i--)
        {
            rightOffset = i;
        }

        var usedValue = rightOffset - leftOffset;

        if (usedValue > 0)
        {
            AutoContrast(rightOffset, leftOffset);
        }
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\SystemImage\\histogram.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);
        return fullFileName;
    }

    #endregion

    #region Private methods
    
    private void AutoContrast(int rightOffset, int leftOffset)
    {
        for (var i = 0; i < Header.Width * Header.Height * Header.PixelSize; i += 3)
        {
            var value1 = (Data[i].Equals(Double.NaN) || Data[i] < 0)? 0 : Data[i];
            var value2 = (Data[i + 1].Equals(Double.NaN) || Data[i] < 0)? 0 : Data[i + 1];
            var value3 = (Data[i + 2].Equals(Double.NaN) || Data[i] < 0)? 0 : Data[i + 2];
            ConvertColorPixel(tempPixel, value1, value2, value3, ColorSpace.Rgb);
            var curColorSpace = _currentColorSpace;
            _currentColorSpace = ColorSpace.Rgb;
            value1 = Math.Round(tempPixel[0] * 255);
            value2 = Math.Round(tempPixel[1] * 255);
            value3 = Math.Round(tempPixel[2] * 255);
            
            value1 = (value1 - leftOffset) / (rightOffset - leftOffset);
            value2 = (value2 - leftOffset) / (rightOffset - leftOffset);
            value3 = (value3 - leftOffset) / (rightOffset - leftOffset);
            value1 = value1 < 0 ? 0 : value1;
            value2 = value2 < 0 ? 0 : value2;
            value3 = value3 < 0 ? 0 : value3;
            value1 = value1 > 1 ? 1 : value1;
            value2 = value2 > 1 ? 1 : value2;
            value3 = value3 > 1 ? 1 : value3;
            
            ConvertColorPixel(tempPixel, value1, value2, value3, curColorSpace);
            _currentColorSpace = curColorSpace;
            Data[i] = tempPixel[0];
            Data[i + 1] = tempPixel[1];
            Data[i + 2] = tempPixel[2];
        }
    }

    private void ConvertColorPixel(double[] pixel, double value1, double value2, double value3, ColorSpace colorSpace)
    {
        if (_currentColorSpace == ColorSpace.Rgb && colorSpace == ColorSpace.Rgb)
        {
            pixel[0] = value1;
            pixel[1] = value2;
            pixel[2] = value3;
            return;
        }

        if (_currentColorSpace == ColorSpace.Rgb)
        {
            switch (colorSpace)
            {
                case ColorSpace.Hsl:
                {
                    RgbToHsl(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.Hsv:
                {
                    RgbToHsv(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr601:
                {
                    RgbToYСbСr601(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr709:
                {
                    RgbToYСbСr709(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    RgbToYCoCg(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.Cmy:
                {
                    RgbToCmy(pixel, value1, value2, value3);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace), colorSpace, null);
            }
        }
        else
        {
            switch (_currentColorSpace)
            {
                case ColorSpace.Hsl:
                {
                    HslToRgb(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.Hsv:
                {
                    HsvToRgb(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr601:
                {
                    YСbСr601ToRgb(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YCbCr709:
                {
                    YСbСr709ToRgb(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.YСoCg:
                {
                    YCoCgToRgb(pixel, value1, value2, value3);
                    break;
                }
                case ColorSpace.Cmy:
                {
                    CmyToRgb(pixel, value1, value2, value3);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(colorSpace), colorSpace, null);
            }
            pixel[0] = (Convert.ToInt32(pixel[0] * 255))/255.0;
            pixel[1] = (Convert.ToInt32(pixel[1] * 255))/255.0;
            pixel[2] = (Convert.ToInt32(pixel[2] * 255))/255.0;
        }
    }

    private void RgbToCmy(double[] pixel, double red, double green, double blue)
    {
        pixel[0] = 1 - red;
        pixel[1] = 1 - green;
        pixel[2] = 1 - blue;
    }

    private void CmyToRgb(double[] pixel, double cyan, double magenta, double yellow)
    {
        pixel[0] = 1 - cyan;
        pixel[1] = 1 - magenta;
        pixel[2] = 1 - yellow;
    }

    private void RgbToHsl(double[] pixel, double red, double green, double blue)
    {
        double maxv = Math.Max(green, Math.Max(red, blue));
        double minv = Math.Min(green, Math.Min(red, blue));
        pixel[2] = (maxv + minv) / 2;
        pixel[1] = CountS(pixel[2], maxv, minv);
        pixel[0] = CountH(red, green, blue, minv, maxv);
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

    private void HslToRgb(double[] pixel, double h, double s, double l)
    {
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

    private void HsvToRgb(double[] pixel, double h, double s, double v)
    {
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
    }

    // переделанные коэффициент-переводы в .709
    private void RgbToYСbСr709(double[] pixel, double red, double green, double blue)
    {

        pixel[0] = (0.2126 * red*255) + (0.7152 * green * 255) + (0.0722 * blue * 255);
        pixel[0] /= 255;
        pixel[1] = 128 - (0.1146 * red*255) - (0.3854 * green*255) + (0.5 * blue*255);
        pixel[1] /= 255;
        pixel[2] = 128 + (0.5 * red*255) - (0.4542 * green*255) - (0.0458 * blue*255);
        pixel[2] /= 255;
    }
    
    private void RgbToYСbСr601(double[] pixel, double red, double green, double blue)
    {
        pixel[0] = (0.299 * red*255) + (0.587 * green*255) + (0.114 * blue*255);
        pixel[0] /= 255;
        pixel[1] = 128 - (0.168736 * red*255) - (0.331264 * green*255) + (0.5 * blue*255);
        pixel[1] /= 255;
        pixel[2] = 128 + (0.5 * red*255) - (0.418688 * green*255) - (0.081312 * blue*255);
        pixel[2] /= 255;
    }

    // переделанные коэффициент-переводы из .709
    private void YСbСr709ToRgb(double[] pixel, double y, double Cb, double Cr)
    {

        pixel[0] = y*255 + 1.5748 * (Cr * 255 - 128);
        pixel[0] /= 255;
        pixel[1] = y*255 - 0.1873 * (Cb * 255 - 128) - 0.4681 * (Cr * 255 - 128);
        pixel[1] /= 255;
        pixel[2] = y*255 + 1.8556 * (Cb * 255 - 128);
        pixel[2] /= 255;
    }

    private void YСbСr601ToRgb(double[] pixel, double y, double Cb, double Cr)
    {
        pixel[0] = y*255 + 1.402 * (Cr * 255 - 128);
        pixel[0] /= 255;
        pixel[1] = y*255 - 0.34414 * (Cb * 255 - 128) - 0.71414 * (Cr * 255 - 128);
        pixel[1] /= 255;
        pixel[2] = y*255 + 1.772 * (Cb * 255 - 128);
        pixel[2] /= 255;
    }

    private void RgbToYCoCg(double[] pixel, double red, double green, double blue)
    {
        // Y [0; 1] Co and Cg [-0.5; 0.5]
        pixel[0] = 0.25 * red + 0.5 * green + 0.25 * blue;
        pixel[1] = 0.5 * red - 0.5 * blue;
        pixel[2] = -0.25 * red + 0.5 * green - 0.25 * blue;
    }

    private void YCoCgToRgb(double[] pixel, double y, double Co, double Cg)
    {
        pixel[0] = y + Co - Cg;
        pixel[1] = y + Cg;
        pixel[2] = y - Co - Cg;
    }
    
    private void SetColorSpace(ColorSpace colorSpace)
    {
        _currentColorSpace = colorSpace;
    }

    #endregion
}