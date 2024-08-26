using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    public static void Main()
    {
        var lexer = new Lexer();

        // Leer el texto de entrada desde un archivo
        string filePath = @"D:\Compilador desde 0\newtest.txt"; 
        string text = File.ReadAllText(filePath);

        var tokens = lexer.Tokenize(text);

        // Imprimir la lista de tokens en la consola
        Console.WriteLine("Tokens generados:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}

