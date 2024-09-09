using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        // Define la ruta de la carpeta donde se encuentran los archivos de código fuente.
        string carpetaRuta = @"D:\Compilador desde 0\Pruebas";
        string[] archivos = Directory.GetFiles(carpetaRuta, "*.txt");

        if (archivos.Length == 0)
        {
            Console.WriteLine("No se encontraron archivos de prueba.");
            return;
        }

        OptimizedAST optimizedAST = new OptimizedAST(); // Instancia para optimizar subárboles de funciones

        foreach (var archivo in archivos)
        {
            Console.WriteLine($"Procesando archivo: {Path.GetFileName(archivo)}");
            string texto = File.ReadAllText(archivo); // Lee el archivo

            try
            {
                // Proceso de tokenización
                List<Token> tokens = Lexer.Tokenize(texto);
                ImprimirTokens(tokens); // Imprime los tokens generados

                // Proceso de parsing
                Parser parser = new Parser(tokens);
                List<ASTNode> ast = parser.Parse();

                // Optimiza el AST antes del análisis semántico
                optimizedAST.AnalyzeAndOptimize(ast);

                // Realiza el análisis semántico en cada nodo del AST
                SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(optimizedAST);
                foreach (var node in ast)
                {
                    node.Accept(semanticAnalyzer);
                }

                Console.WriteLine($"Archivo {Path.GetFileName(archivo)} procesado exitosamente.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando {Path.GetFileName(archivo)}: {ex.Message}");
            }
        }
    }

    // Función para imprimir la lista de tokens generada por el lexer
    static void ImprimirTokens(List<Token> tokens)
    {
        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
