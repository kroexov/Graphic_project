using System;
using System.Drawing;
using System.Drawing.Imaging;

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
    public string OrderedAlgorithm(string oldimagepath, int bitn)
    {
        Bitmap oldimage = (Bitmap) Image.FromFile(oldimagepath);
        int width = oldimage.Width;
        int height = oldimage.Height;
        
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
                
                Color newColor = Color.FromArgb(value1,
                    value2, 
                    value3);
                
                image.SetPixel(x, y, newColor);
            }
            
        }
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + "dithered.bmp";
        image.Save(fullFileName, ImageFormat.Bmp);

        return fullFileName;
    }

    
    
    protected int GetCoordinates(int width, int x, int y)
    {
        return y * width + x;
    }
}