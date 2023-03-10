using System;
using DynamicData.Binding;

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
            if (result > 255)
            {
                result = 255;
            }
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
            if (result < 0)
            {
                result = 0;
            }
        }
        return (byte)result;
    }

    public static byte CastToClosest(this byte value, int bitn)
    {
        int step = 255 / ((int) Math.Pow(2, bitn) - 1);
        int result = 0;
        while (result < value)
        {
            result += step;
            if (result > 255)
            {
                result = 255;
            }
        }

        int prev_result = result - step;
        if (Math.Abs(result - value) <= Math.Abs(prev_result - value))
        {
            return (byte)result;
        }

        return (byte)prev_result;
    }
}