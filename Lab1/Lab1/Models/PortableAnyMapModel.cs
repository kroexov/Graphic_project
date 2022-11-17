using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Lab1.Models;

namespace Lab1.Models;

public class PortableAnyMapModel
{
    #region Private fields

    public bool ColorType = true;
    private byte[] _bytes;
    private byte[] _bytesOfImage;
    private int _index;
    private FileHeaderInfo _header;

    #endregion

    #region Private methods

    private string ExtractHeaderInfo()
    {
        var header = "";
        var lineBreakCounter = 0;
        const int codeOfLineBreakChar = 10;
        _index = 0;

        while (lineBreakCounter != 3)
        {
            if (_bytes[_index] == codeOfLineBreakChar)
            {
                lineBreakCounter++;
                header += " ";
            }
            else
            {
                header += Convert.ToChar(_bytes[_index]);
            }

            _index++;
        }

        return header;
    }

    private void ExtractImageBytes()
    {
        _bytesOfImage = new byte[_header.Width * _header.Height * _header.PixelSize];
        for (var i = 0; i < _header.Width * _header.Height * _header.PixelSize; i++)
        {
            _bytesOfImage[i] = _bytes[i + _index];
        }
    }
    
    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        (lhs, rhs) = (rhs, lhs);
    }

    private void ChangeToBgr()
    {
        for (var i = 0; i < _bytesOfImage.Length - 2; i += 3)
        {
            Swap(ref _bytesOfImage[i], ref _bytesOfImage[i + 2]);
        }
    }

    #endregion

    #region Public methods

    public void ReadFile(String filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        _bytes = File.ReadAllBytes(filePath);
        var headerInfo = ExtractHeaderInfo();
        _header = new FileHeaderInfo(headerInfo);
        ExtractImageBytes();

        if (_header.Width * _header.Height * _header.PixelSize > _bytesOfImage.Length)
        {
            throw new Exception("Damaged file");
        }
    }

    public string AfterOpenFileLogic(string filePath)
    {
        var cr = new BitmapCreate();
        var newImage = new Bitmap(3, 3);
        var fileName = Path.GetFileName(filePath);
        fileName = fileName.Substring(0, fileName.Length - 3) + "bmp";
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + fileName;

        switch (_header.FileFormat)
        {
            case "P6" when _header.MaxColorLevel == 255:
            {
                if (!ColorType)
                {
                    ChangeToBgr();
                }

                newImage = cr.CreateP6Bit8(_header, _bytesOfImage);
                break;
            }
            case "P5" when _header.MaxColorLevel == 255:
            {
                newImage = cr.CreateP5Bit8(_header, _bytesOfImage);
                break;
            }
        }

        newImage.Save(fullFileName, ImageFormat.Bmp);
        return fullFileName;
    }

    #endregion

    #region Events

    public event Action<string>? ModelErrorHappened;

    #endregion
}