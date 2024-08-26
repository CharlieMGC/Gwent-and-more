using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        string rutaArchivo = @"D:\Compilador desde 0\newtest.txt";
        string texto = File.ReadAllText(rutaArchivo);
        var tokens = Lexer(texto);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine($"Tipo: {token.Tipo}, Valor: {token.Valor}");
        }
    }

    static List<Token> Lexer(string texto)
    {
        List<Token> tokens = new List<Token>();
        string tokenActual = string.Empty;

        foreach (char c in texto)
        {
            if (char.IsLetterOrDigit(c))
            {
                tokenActual += c;
            }
            else
            {
                if (!string.IsNullOrEmpty(tokenActual))
                {
                    tokens.Add(new Token { Tipo = "Palabra", Valor = tokenActual });
                    tokenActual = string.Empty;
                }
                if (!char.IsWhiteSpace(c))
                {
                    tokens.Add(new Token { Tipo = "Signo", Valor = c.ToString() });
                }
            }
        }

        if (!string.IsNullOrEmpty(tokenActual))
        {
            tokens.Add(new Token { Tipo = "Palabra", Valor = tokenActual });
        }

        return tokens;
    }
}

class Token
{
    public string? Tipo { get; set; }
    public string? Valor { get; set; }
}
