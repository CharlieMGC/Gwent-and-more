using System;
using System.Collections.Generic;
using System.IO;

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

        // Crear el parser con los tokens generados
        Parser parser = new Parser(tokens);
        ASTNode ast = parser.Parse();

        // Imprimir el AST
        Console.WriteLine("AST:");
        PrintAST(ast, 0);
    }

    static void PrintAST(ASTNode node, int indent)
    {
        if (node == null) return;

        string indentString = new string(' ', indent);
        Console.WriteLine(indentString + node.GetType().Name);

        if (node is BinaryExpression binary)
        {
            PrintAST(binary.Left, indent + 2);
            Console.WriteLine(indentString + "  " + binary.Operator);
            PrintAST(binary.Right, indent + 2);
        }
        else if (node is LiteralExpression literal)
        {
            Console.WriteLine(indentString + "  " + literal.Value);
        }
        else if (node is VariableExpression variable)
        {
            Console.WriteLine(indentString + "  " + variable.Name);
        }
        else if (node is GroupingExpression grouping)
        {
            PrintAST(grouping.Expression, indent + 2);
        }
        else if (node is UnaryExpression unary)
        {
            Console.WriteLine(indentString + "  " + unary.Operator);
            PrintAST(unary.Right, indent + 2);
        }
        else if (node is VariableDeclaration variableDeclaration)
        {
            Console.WriteLine(indentString + "  Type: " + variableDeclaration.Type.Lexeme);
            Console.WriteLine(indentString + "  Name: " + variableDeclaration.Name.Lexeme);
            Console.WriteLine(indentString + "  Initializer:");
            PrintAST(variableDeclaration.Initializer, indent + 2);
        }
    }
}
