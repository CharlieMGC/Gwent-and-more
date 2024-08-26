using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string filePath = @"D:\Compilador desde 0\newtest.txt";
        
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            Console.WriteLine("Contenido del archivo:");
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine("El archivo no existe.");
        }
    }

}