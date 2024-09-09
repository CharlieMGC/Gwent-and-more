using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string carpetaRuta = @"D:\Gwent-and-more\Pruebas";
        
        string[] archivos = Directory.GetFiles(carpetaRuta, "*.txt");

        if (archivos.Length == 0)
        {
            Console.WriteLine("No se encontraron archivos de prueba en la carpeta especificada.");
            return;
        }

        foreach (var archivo in archivos)
        {
            Console.WriteLine($"Procesando archivo: {Path.GetFileName(archivo)}");

            string texto;
            try
            {
                texto = File.ReadAllText(archivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo {archivo}: {ex.Message}");
                continue;
            }

            // Tokenización
            List<Token> tokens;
            try
            {
                tokens = Lexer.Tokenize(texto);
                ImprimirTokens(tokens);
            }
            catch (LexerException ex)
            {
                Console.WriteLine($"Error durante la tokenización: {ex.Message}");
                continue;
            }

            // Parsing
            var parser = new Parser(tokens);
            List<ASTNode> ast;
            try
            {
                ast = parser.Parse().Where(node => node != null).ToList();
            }
            catch (ParserException ex)
            {
                Console.WriteLine($"Error al parsear: {ex.Message}");
                continue;
            }

            // Imprimir el AST utilizando ASTPrinter
            Console.WriteLine("\nAST:");
            var printer = new ASTPrinter();
            foreach (var node in ast)
            {
                string result = node.Accept(printer);
                Console.WriteLine(result);
                Console.WriteLine(); // Línea en blanco para separar nodos
            }


            // Análisis Semántico
            Console.WriteLine("Iniciando análisis semántico...");
            var semanticAnalyzer = new SemanticAnalyzer();
            try
            {
                foreach (var node in ast)
                {
                    node.Accept(semanticAnalyzer);
                }
                Console.WriteLine("Análisis semántico completado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el análisis semántico: {ex.Message}");
                continue;
            }

            Console.WriteLine($"Archivo {Path.GetFileName(archivo)} procesado exitosamente.\n");
        }

        Console.WriteLine("Todos los archivos han sido procesados.");
    }

    static void ImprimirTokens(List<Token> tokens)
    {
        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
