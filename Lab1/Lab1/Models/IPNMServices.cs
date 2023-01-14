namespace Lab1.Models;

public interface IPNMServices
{
    // Читает файл по пути и создает объект класса ImgFile
    void ReadFile(string filePath);

    void ChangeColorSpace(ColorSpace newColorSpace);

    void ChangeColorChannel(bool[] newColorChannel);
}