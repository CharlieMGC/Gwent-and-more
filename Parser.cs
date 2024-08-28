// Parser.cs
using System;
using System.Collections.Generic;

public class ParserException : Exception
{
    public ParserException(string message) : base(message) { }
}

public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public ASTNode Parse()
    {
        return Expression();
    }

    private ASTNode Expression()
    {
        Console.WriteLine("Parsing Expression");
        return Equality();
    }

    private ASTNode Equality()
    {
        Console.WriteLine("Parsing Equality");
        ASTNode expr = Comparison();

        while (Match(TokenType.EQUAL, TokenType.NOT_EQUAL))
        {
            Token op = Previous();
            ASTNode right = Comparison();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Comparison()
    {
        Console.WriteLine("Parsing Comparison");
        ASTNode expr = Term();

        while (Match(TokenType.GREATER_THAN, TokenType.GREATER_EQUAL, TokenType.LESS_THAN, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            ASTNode right = Term();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Term()
    {
        Console.WriteLine("Parsing Term");
        ASTNode expr = Factor();

        while (Match(TokenType.PLUS, TokenType.MINUS))
        {
            Token op = Previous();
            ASTNode right = Factor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Factor()
    {
        Console.WriteLine("Parsing Factor");
        ASTNode expr = Unary();

        while (Match(TokenType.TIMES, TokenType.DIVIDE))
        {
            Token op = Previous();
            ASTNode right = Unary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Unary()
    {
        Console.WriteLine("Parsing Unary");
        if (Match(TokenType.NOT, TokenType.MINUS))
        {
            Token op = Previous();
            ASTNode right = Unary();
            return new UnaryExpression(op, right);
        }

        return Primary();
    }

    private ASTNode Primary()
    {
        Console.WriteLine($"Parsing Primary: {Peek().Type}");
        
        if (Match(TokenType.NUMBER))
        {
            return new LiteralExpression(Previous());
        }

        if (Match(TokenType.STRING))
        {
            return new LiteralExpression(Previous());
        }

        if (Match(TokenType.TRUE))
        {
            return new LiteralExpression(new Token(TokenType.BOOLEAN, "true", true, Previous().Line));
        }

        if (Match(TokenType.FALSE))
        {
            return new LiteralExpression(new Token(TokenType.BOOLEAN, "false", false, Previous().Line));
        }

        if (Match(TokenType.NULL))
        {
            return new LiteralExpression(new Token(TokenType.NULL, "null", null, Previous().Line));
        }

        if (Match(TokenType.LPAREN))
        {
            ASTNode expr = Expression();
            Consume(TokenType.RPAREN, "Esperaba ')'.");
            return new GroupingExpression(expr);
        }

        if (Match(TokenType.ID))
        {
            return new VariableExpression(Previous());
        }

        if (Match(TokenType.KEYWORD))
        {
            // Manejar palabras clave específicas aquí
            Token keyword = Previous();
            switch (keyword.Lexeme)
            {
                case "if":
                    return ParseIfStatement();
                case "while":
                    return ParseWhileStatement();
                case "for":
                    return ParseForStatement();
                case "return":
                    return ParseReturnStatement();
                case "int":
                case "float":
                case "char":
                case "bool":
                case "void":
                case "string":
                    return ParseVariableDeclaration(keyword);
                // Agregar más palabras clave según sea necesario
                default:
                    throw new ParserException($"Palabra clave no reconocida: {keyword.Lexeme}");
            }
        }

        throw new ParserException($"Esperaba una expresión. Token actual: {Peek().Type}");
    }

    private ASTNode ParseIfStatement()
    {
        // Implementar el análisis de la declaración if
        // ...
        throw new NotImplementedException("ParseIfStatement no está implementado.");
    }

    private ASTNode ParseWhileStatement()
    {
        // Implementar el análisis de la declaración while
        // ...
        throw new NotImplementedException("ParseWhileStatement no está implementado.");
    }

    private ASTNode ParseForStatement()
    {
        // Implementar el análisis de la declaración for
        // ...
        throw new NotImplementedException("ParseForStatement no está implementado.");
    }

    private ASTNode ParseReturnStatement()
    {
        // Implementar el análisis de la declaración return
        // ...
        throw new NotImplementedException("ParseReturnStatement no está implementado.");
    }

    private ASTNode ParseVariableDeclaration(Token keyword)
    {
        // Implementar el análisis de la declaración de variables
        // Ejemplo: int x = 10;
        Token name = Consume(TokenType.ID, "Esperaba un nombre de variable.");
        Consume(TokenType.ASSIGN, "Esperaba '=' después del nombre de la variable.");
        ASTNode initializer = Expression();
        Consume(TokenType.SEMICOLON, "Esperaba ';' después de la declaración de la variable.");
        return new VariableDeclaration(keyword, name, initializer);
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw new ParserException(message);
    }
}
