using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Lab1.Models;

public class DitheringServices
{
    private byte[][] matrix = { 
        new byte[] { 0, 128, 32, 160, 8, 136, 40, 168 }, 
        new byte[] { 192, 64, 224, 96, 200, 72, 232, 104 }, 
        new byte[] { 48, 176, 16, 144, 56, 184, 24, 152 },
        new byte[] { 240, 112, 208, 80, 248, 120, 216, 88 },
        new byte[] { 12, 140, 44, 172, 4, 132, 36, 164 },
        new byte[] { 204, 76, 236, 108, 196, 68, 228, 100 },
        new byte[] { 60, 188, 28, 156, 52, 180, 20, 148 },
        new byte[] { 252, 124, 220, 92, 244, 116, 212, 84 },
    };

    private Random random = new Random();
    public string OrderedAlgorithm(string oldimagepath, int bitn)
    {
        Bitmap oldimage = (Bitmap) Image.FromFile(oldimagepath);
        int width = oldimage.Width;
        int height = oldimage.Height;
        byte[] newData = new byte[width * height * 3];
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        
        var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = oldimage.GetPixel(x, y);
                var matrixh = y % 8; // 81 -> 1
                var matrixw = x % 8; // 79 -> 7
                var value1 = color.R;
                var value2 = color.G;
                var value3 = color.B;
                
                value1 = (value1 > matrix[matrixh][matrixw]) ? value1.CastUp(bitn) : value1.CastDown(bitn);
                value2 = (value2 > matrix[matrixh][matrixw]) ? value2.CastUp(bitn) : value2.CastDown(bitn);
                value3 = (value3 > matrix[matrixh][matrixw]) ? value3.CastUp(bitn) : value3.CastDown(bitn);

                newData[3 * (y * height + x)] = value1;
                newData[3 * (y * height + x) + 1] = value2;
                newData[3 * (y * height + x) + 2] = value3;
                
                Color newColor = Color.FromArgb(value1,
                    value2, 
                    value3);
                
                image.SetPixel(x, y, newColor);
            }
            
        }
        File.WriteAllBytes(pathSaveFile + "\\imgFiles\\" + "ditheredFinal.txt", newData);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "dithered.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);

        return fullFileName;
    }
    
    public string RandomAlgorithm(string oldimagepath, int bitn)
    {
        Bitmap oldimage = (Bitmap) Image.FromFile(oldimagepath);
        int width = oldimage.Width;
        int height = oldimage.Height;
        byte[] newData = new byte[width * height * 3];
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        
        var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = oldimage.GetPixel(x, y);
                var randomNumber = random.Next(0, 256);
                var value1 = color.R;
                var value2 = color.G;
                var value3 = color.B;
                
                value1 = (value1 > randomNumber) ? value1.CastUp(bitn) : value1.CastDown(bitn);
                value2 = (value2 > randomNumber) ? value2.CastUp(bitn) : value2.CastDown(bitn);
                value3 = (value3 > randomNumber) ? value3.CastUp(bitn) : value3.CastDown(bitn);

                newData[3 * (y * height + x)] = value1;
                newData[3 * (y * height + x) + 1] = value2;
                newData[3 * (y * height + x) + 2] = value3;
                
                Color newColor = Color.FromArgb(value1,
                    value2, 
                    value3);
                
                image.SetPixel(x, y, newColor);
            }
            
        }
        File.WriteAllBytes(pathSaveFile + "\\imgFiles\\" + "ditheredFinal.txt", newData);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "dithered.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);

        return fullFileName;
    }

    public string FloydSteinbergAlgorithm(string oldimagepath, int bitn)
    {
        Bitmap oldimage = (Bitmap) Image.FromFile(oldimagepath);
        int width = oldimage.Width;
        int height = oldimage.Height;
        byte[] newData = new byte[width * height * 3];
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        
        var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = oldimage.GetPixel(x, y);
                var value1 = color.R;
                var value2 = color.G;
                var value3 = color.B;

                value1 = value1.CastToClosest(bitn);
                var diffR = color.R - value1;
                
                value2 = value2.CastToClosest(bitn);
                var diffG = color.G - value2;
                
                value3 = value3.CastToClosest(bitn);
                var diffB = color.B - value3;
                
                //Прибавляем ошибки к другим пикселям
                if (x + 1 < width)
                {
                    AddErrorToPixel(x + 1, y, 7.0 / 16, diffR, diffG, diffB, oldimage);
                    // pixel[y][x+1] += (7.0 / 16) * diff1;
                }

                if (y + 1 < height)
                {
                    if (x - 1 >= 0)
                    {
                        AddErrorToPixel(x - 1, y + 1, 3.0 / 16, diffR, diffG, diffB, oldimage);
                        //pixel[y+1][x-1] += (3.0 / 16) * diff1;
                    }
                    AddErrorToPixel(x, y, 5.0 / 16, diffR, diffG, diffB, oldimage);
                    // pixel[y+1][x] += (5.0 / 16) * diff1;
                    if (x + 1 < width)
                    {
                        AddErrorToPixel(x + 1, y + 1, 1.0 / 16, diffR, diffG, diffB, oldimage);
                        //pixel[y+1][x+1] += (1.0 / 16) * diff1;
                    }
                    
                }

                newData[3 * (y * height + x)] = value1;
                newData[3 * (y * height + x) + 1] = value2;
                newData[3 * (y * height + x) + 2] = value3;
                
                Color newColor = Color.FromArgb(value1,
                    value2, 
                    value3);
                
                image.SetPixel(x, y, newColor);
            }
            
        }
        File.WriteAllBytes(pathSaveFile + "\\imgFiles\\" + "ditheredFinal.txt", newData);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "dithered.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);

        return fullFileName;
    }

    private byte ConvertValueToByte(double value)
    {
        if (value < 0)
        {
            return Convert.ToByte(0);
        }

        if (value > 255)
        {
            return Convert.ToByte(255);
        }

        return Convert.ToByte(value);
    }

    private void AddErrorToPixel(int x, int y, double coef, int diffR, int diffG, int diffB, Bitmap oldImage)
    {
        var tmpColor = oldImage.GetPixel(x, y);
        var tmpValue1 = tmpColor.R + coef * diffR;
        byte tmpValue11 = ConvertValueToByte(tmpValue1);
        var tmpValue2 = tmpColor.G + coef * diffG;
        byte tmpValue22 = ConvertValueToByte(tmpValue2);
        var tmpValue3 = tmpColor.B + coef * diffB;
        byte tmpValue33 = ConvertValueToByte(tmpValue3);
                    
        tmpColor = Color.FromArgb(tmpValue11,
            tmpValue22, 
            tmpValue33);
                
        oldImage.SetPixel(x, y, tmpColor);
    }
    
    public string AtkinsonAlgorithm(string oldimagepath, int bitn)
    {
        Bitmap oldimage = (Bitmap) Image.FromFile(oldimagepath);
        int width = oldimage.Width;
        int height = oldimage.Height;
        byte[] newData = new byte[width * height * 3];
        
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        
        var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var color = oldimage.GetPixel(x, y);
                var value1 = color.R;
                var value2 = color.G;
                var value3 = color.B;

                value1 = value1.CastToClosest(bitn);
                var diffR = color.R - value1;
                
                value2 = value2.CastToClosest(bitn);
                var diffG = color.G - value2;
                
                value3 = value3.CastToClosest(bitn);
                var diffB = color.B - value3;

                double coef = 1.0 / 8;
                
                //Прибавляем ошибки к другим пикселям
                if (x + 1 < width)
                {
                    AddErrorToPixel(x + 1, y, coef, diffR, diffG, diffB, oldimage);
                }
                
                if (x + 2 < width)
                {
                    AddErrorToPixel(x + 2, y, coef, diffR, diffG, diffB, oldimage);
                }

                if (y + 1 < height)
                {
                    if (x - 1 >= 0)
                    {
                        AddErrorToPixel(x - 1, y + 1, coef, diffR, diffG, diffB, oldimage);
                    }
                    
                    AddErrorToPixel(x, y, coef, diffR, diffG, diffB, oldimage);

                    if (x + 1 < width)
                    {
                        AddErrorToPixel(x + 1, y + 1, coef, diffR, diffG, diffB, oldimage);
                    }
                }

                if (y + 2 < height)
                {
                    AddErrorToPixel(x, y + 2, coef, diffR, diffG, diffB, oldimage);
                }

                newData[3 * (y * height + x)] = value1;
                newData[3 * (y * height + x) + 1] = value2;
                newData[3 * (y * height + x) + 2] = value3;

                Color newColor = Color.FromArgb(value1,
                    value2, 
                    value3);
                
                image.SetPixel(x, y, newColor);
            }
            
        }
        File.WriteAllBytes(pathSaveFile + "\\imgFiles\\" + "ditheredFinal.txt", newData);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "dithered.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);

        return fullFileName;
    }

    protected int GetCoordinates(int width, int x, int y)
    {
        return y * width + x;
    }
}