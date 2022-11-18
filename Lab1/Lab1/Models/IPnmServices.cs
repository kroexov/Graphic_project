namespace Lab1.Models;

public interface IPnmServices
{
    // Читает файл по пути и создает объект класса ImgFile
    string ReadFile(string filePath);

    void ChangeColorSpace(ColorSpace newColorSpace);

    void ChangeColorChannel(bool[] newColorChannel);
}