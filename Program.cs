using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        string rutaArchivo = @"D:\Compilador desde 0\newtest.txt";
        string texto;

        try
        {
            texto = File.ReadAllText(rutaArchivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
            return;
        }

        var tokens = Lexer.Tokenize(texto);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        // Crear el parser con los tokens generados
        Parser parser = new Parser(tokens);
        List<ASTNode> ast;

        try
        {
            ast = parser.Parse();
        }
        catch (ParserException ex)
        {
            Console.WriteLine($"Error al parsear: {ex.Message}");
            return;
        }

        // Imprimir el AST
        Console.WriteLine("AST:");
        foreach (var node in ast)
        {
            PrintAST(node, 0);
        }
    }

    static void PrintAST(ASTNode node, int indent)
    {
        if (node == null) return;

        string indentString = new string(' ', indent);
        Console.WriteLine(indentString + node.GetType().Name);

        switch (node)
        {
            case BinaryExpression binary:
                PrintAST(binary.Left, indent + 2);
                Console.WriteLine(indentString + "  " + binary.Operator.Type);
                                PrintAST(binary.Right, indent + 2);
                break;
            case LiteralExpression literal:
                Console.WriteLine(indentString + "  " + literal.Value);
                break;
            case VariableExpression variable:
                Console.WriteLine(indentString + "  " + variable.Name.Lexeme);
                break;
            case GroupingExpression grouping:
                PrintAST(grouping.Expression, indent + 2);
                break;
            case UnaryExpression unary:
                Console.WriteLine(indentString + "  " + unary.Operator.Type);
                PrintAST(unary.Right, indent + 2);
                break;
            case VariableDeclaration variableDeclaration:
                Console.WriteLine(indentString + "  Type: " + variableDeclaration.Type.Lexeme);
                Console.WriteLine(indentString + "  Name: " + variableDeclaration.Name.Lexeme);
                Console.WriteLine(indentString + "  Initializer:");
                PrintAST(variableDeclaration.Initializer, indent + 2);
                break;
            case FunctionDeclaration functionDeclaration:
                Console.WriteLine(indentString + "  Function: " + functionDeclaration.Name.Lexeme);
                Console.WriteLine(indentString + "  Parameters:");
                foreach (var param in functionDeclaration.Parameters)
                {
                    PrintAST(param, indent + 2);
                }
                Console.WriteLine(indentString + "  Body:");
                PrintAST(functionDeclaration.Body, indent + 2);
                break;
            case ExpressionStatement expressionStatement:
                PrintAST(expressionStatement.Expression, indent + 2);
                break;
            case IfStatement ifStatement:
                Console.WriteLine(indentString + "  Condition:");
                PrintAST(ifStatement.Condition, indent + 2);
                Console.WriteLine(indentString + "  Then:");
                PrintAST(ifStatement.ThenBranch, indent + 2);
                if (ifStatement.ElseBranch != null)
                {
                    Console.WriteLine(indentString + "  Else:");
                    PrintAST(ifStatement.ElseBranch, indent + 2);
                }
                break;
            case WhileStatement whileStatement:
                Console.WriteLine(indentString + "  Condition:");
                PrintAST(whileStatement.Condition, indent + 2);
                Console.WriteLine(indentString + "  Body:");
                PrintAST(whileStatement.Body, indent + 2);
                break;
            case ForStatement forStatement:
                Console.WriteLine(indentString + "  Initializer:");
                PrintAST(forStatement.Initializer, indent + 2);
                Console.WriteLine(indentString + "  Condition:");
                PrintAST(forStatement.Condition, indent + 2);
                Console.WriteLine(indentString + "  Increment:");
                PrintAST(forStatement.Increment, indent + 2);
                Console.WriteLine(indentString + "  Body:");
                PrintAST(forStatement.Body, indent + 2);
                break;
            case BlockStatement blockStatement:
                Console.WriteLine(indentString + "  Block:");
                foreach (var stmt in blockStatement.Statements)
                {
                    PrintAST(stmt, indent + 2);
                }
                break;
            case ReturnStatement returnStatement:
                Console.WriteLine(indentString + "  Return:");
                PrintAST(returnStatement.Value, indent + 2);
                break;
            default:
                Console.WriteLine(indentString + "  Nodo no reconocido");
                break;
        }
    }
}
