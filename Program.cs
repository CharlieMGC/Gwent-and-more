using System;
using System.Collections.Generic;
using System.IO;
using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string rutaArchivo = @"D:\Compilador desde 0\newtest.txt";
        string texto = File.ReadAllText(rutaArchivo);
        var tokens = Lexer.Tokenizer(texto);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}