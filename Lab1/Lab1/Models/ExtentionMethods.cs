using System;

namespace Lab1.Models;

public static class ExtentionMethods
{
    public static byte CastUp(this byte value, int bitn)
    {
        int step = 255 / ((int) Math.Pow(2, bitn) - 1);
        int result = 0;
        while (result < value)
        {
            result += step;
        }
        return (byte)result;
    }
    
    public static byte CastDown(this byte value, int bitn)
    {
        int step = 255 / ((int) Math.Pow(2, bitn) - 1);
        int result = 255;
        while (result > value)
        {
            result -= step;
        }
        return (byte)result;
    }
}