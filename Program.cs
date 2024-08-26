using System;
using System.Collections.Generic;
using System.IO;

public class Program
{
    public static void Main()
    {
        var lexer = new Lexer();

        string filePath = @"D:\Compilador desde 0\newtest.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"El archivo no existe: {filePath}");
            return;
        }

        string text = File.ReadAllText(filePath);

        try
        {
            var tokens = lexer.Tokenize(text);

            Console.WriteLine("Tokens generados:");
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
