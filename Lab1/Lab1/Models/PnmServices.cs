using System;
using System.Drawing.Imaging;
using System.IO;
using Lab1.TypeFileImg;

namespace Lab1.Models;

public class PnmServices: IPnmServices
{
    #region Private fields

    private TypeFile _typeFile;
    private byte[] _bytes;
    private string _filePath;
    private Pnm _fileImg;

    #endregion

    #region Public methods

    public string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }
        
        _bytes = File.ReadAllBytes(filePath);
        _typeFile = FindTypeFile();
        _filePath = filePath;
        _fileImg = FindPnmImg();

        var fileName = Path.GetFileName(filePath);
        fileName = fileName.Substring(0, fileName.Length - 3) + "bmp";
        var pathSaveFile = AppDomain.CurrentDomain.BaseDirectory;
        pathSaveFile = pathSaveFile.Substring(0, pathSaveFile.Length - 17);
        var fullFileName = pathSaveFile + "\\imgFiles\\" + fileName;
        var test  = _fileImg.CreateBitmap();
        test.Save(fullFileName, ImageFormat.Bmp);
        return fullFileName;
        
        
    }

    public void ChangeColorSpace(ColorSpace newColorSpace)
    {
        if (_fileImg != null)
        {
            _fileImg.ConvertColor(newColorSpace);
        }
    }

    public void ChangeColorChannel(bool[] newColorChannel)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Private methods

    private TypeFile FindTypeFile()
    {
        var res = "";
        for (var i = 0; i < 16; i++)
        {
            res += Convert.ToChar(_bytes[i]);
        }

        if (res[0] == 'P')
        {
            if (res[1] == '5')
                return TypeFile.P5;
            
            if (res[1] == '6')
                return TypeFile.P6;
        }
        else if (res[1] == 'P' && res[1] == 'N' && res[1] == 'G')
        {
            return TypeFile.Png;
        }

        return TypeFile.IsFalseFile;
    }

    private Pnm FindPnmImg()
    {
        switch (_typeFile)
        {
            case TypeFile.P5:
            {
                return new P5(_bytes);
            }
            case TypeFile.P6:
            {
                return new P6(_bytes);
            }
        }

        return null;
    }

    #endregion
    
    #region Events

    public event Action<string>? ModelErrorHappened;

    #endregion
}