using System;
using System.Net;

namespace Lab1.Models;

public class FileHeaderInfo
{
    #region Constructor

    public FileHeaderInfo(string header)
    {
        string[] headerItems = header.Trim().Split();
        if (headerItems.Length != 4)
        {
            throw new Exception("Damaged file: header doesn't match required format");
        }

        FileFormat = headerItems[0];
        Width = Convert.ToInt32(headerItems[1]);
        Height = Convert.ToInt32(headerItems[2]);
        MaxColorLevel = Convert.ToInt32(headerItems[3]);
        PixelSize = 1;
        if (FileFormat.Equals("P6"))
        {
            PixelSize *= 3;
        }
    }

    #endregion
    
    #region Public properties

    public string FileFormat { get; }
    public int Width { get; }
    public int Height { get; }
    public int MaxColorLevel { get; }
    public int PixelSize { get; }

    #endregion
}