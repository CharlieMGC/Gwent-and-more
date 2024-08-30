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

    public List<ASTNode> Parse()
    {
        List<ASTNode> nodes = new List<ASTNode>();
        while (!IsAtEnd())
        {
            nodes.Add(Expression());
        }
        return nodes;
    }
    private ASTNode Expression()
    {
        Console.WriteLine("Parsing Expression");
        ASTNode expr = Equality();
        Console.WriteLine($"Expression parsed: {expr.GetType().Name}");
        return expr;
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
            Console.WriteLine($"Equality parsed: {expr.GetType().Name}");
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
            Console.WriteLine($"Comparison parsed: {expr.GetType().Name}");
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
            Console.WriteLine($"Term parsed: {expr.GetType().Name}");
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
            Console.WriteLine($"Factor parsed: {expr.GetType().Name}");
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
            Console.WriteLine($"Unary parsed: {op.Type}");
            return new UnaryExpression(op, right);
        }

        return Primary();
    }
    private ASTNode Primary()
    {
        Console.WriteLine($"Parsing Primary: {Peek().Type}");

        if (Match(TokenType.NUMBER))
        {
            Console.WriteLine("Primary: Number");
            return new LiteralExpression(Previous());
        }

        if (Match(TokenType.STRING))
        {
            Console.WriteLine("Primary: String");
            return new LiteralExpression(Previous());
        }

        if (Match(TokenType.TRUE))
        {
            Console.WriteLine("Primary: True");
            return new LiteralExpression(new Token(TokenType.BOOLEAN, "true", true, Previous().Line));
        }

        if (Match(TokenType.FALSE))
        {
            Console.WriteLine("Primary: False");
            return new LiteralExpression(new Token(TokenType.BOOLEAN, "false", false, Previous().Line));
        }

        if (Match(TokenType.NULL))
        {
            Console.WriteLine("Primary: Null");
            return new LiteralExpression(new Token(TokenType.NULL, "null", null, Previous().Line));
        }

        if (Match(TokenType.LPAREN))
        {
            Console.WriteLine("Primary: Grouping - Found '('");
            ASTNode expr = Expression();
            Console.WriteLine("Primary: Grouping - Expecting ')'");
            Consume(TokenType.RPAREN, "Esperaba ')'.");
            Console.WriteLine("Primary: Grouping - Found ')'");
            return new GroupingExpression(expr);
        }

        if (Match(TokenType.ID))
        {
            Console.WriteLine("Primary: Identifier");
            return new VariableExpression(Previous());
        }

        if (Match(TokenType.KEYWORD))
        {
            // Manejar palabras clave específicas aquí
            Token keyword = Previous();
            Console.WriteLine($"Primary: Keyword {keyword.Lexeme}");
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
        Token name = Consume(TokenType.ID, "Esperaba un nombre de variable.");
        Consume(TokenType.ASSIGN, "Esperaba '=' después del nombre de la variable.");
        ASTNode initializer = Expression();
        Consume(TokenType.SEMICOLON, "Esperaba ';' después de la declaración de la variable.");
        Console.WriteLine($"VariableDeclaration: {name.Lexeme}");
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
        Console.WriteLine($"Avanzando a token: {Peek().Type}");
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
        if (Check(type))
        {
            Console.WriteLine($"Consuming token: {Peek().Type}");
            return Advance();
        }
        throw new ParserException(message);
    }
}
