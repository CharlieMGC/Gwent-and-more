using System;
using System.Collections.Generic;

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
        List<ASTNode> statements = new List<ASTNode>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    private ASTNode Declaration()
    {
        try
        {
            if (Match(TokenType.FUN)) return FunctionDeclaration();
            if (Match(TokenType.TYPE)) return VariableDeclaration();
            return Statement();
        }
        catch (ParserException)
        {
            Synchronize();
            return null;
        }
    }

    private ASTNode FunctionDeclaration()
    {
        Token name = Consume(TokenType.ID, "Expected function name.");
        Consume(TokenType.LPAREN, "Expected '(' after function name.");

        List<VariableDeclaration> parameters = new List<VariableDeclaration>();
        if (!Check(TokenType.RPAREN))
        {
            do
            {
                Token paramType = Consume(TokenType.TYPE, "Expected parameter type.");
                Token paramName = Consume(TokenType.ID, "Expected parameter name.");
                parameters.Add(new VariableDeclaration(paramType, paramName, null));
            } while (Match(TokenType.COMMA));
        }
        Consume(TokenType.RPAREN, "Expected ')' after parameters.");
        Consume(TokenType.LBRACE, "Expected '{' before function body.");

        BlockStatement body = Block();
        return new FunctionDeclaration(name, parameters, body);
    }

    private VariableDeclaration VariableDeclaration()
    {
        Token type = Previous();
        Token name = Consume(TokenType.ID, "Expected variable name.");

        ASTNode? initializer = null;
        if (Match(TokenType.ASSIGN))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration.");
        return new VariableDeclaration(type, name, initializer);
    }

    private ASTNode Statement()
    {
        if (Match(TokenType.IF)) return IfStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.FOR)) return ForStatement();
        if (Match(TokenType.RETURN)) return ReturnStatement();
        if (Match(TokenType.LBRACE)) return Block();
        return ExpressionStatement();
    }

    private ASTNode IfStatement()
    {
        Consume(TokenType.LPAREN, "Expected '(' after 'if'.");
        ASTNode condition = Expression();
        Consume(TokenType.RPAREN, "Expected ')' after if condition.");

        ASTNode thenBranch = Statement();
        ASTNode elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private ASTNode WhileStatement()
    {
        Consume(TokenType.LPAREN, "Expected '(' after 'while'.");
        ASTNode condition = Expression();
        Consume(TokenType.RPAREN, "Expected ')' after condition.");
        ASTNode body = Statement();

        return new WhileStatement(condition, body);
    }

    private ASTNode ForStatement()
    {
        Consume(TokenType.LPAREN, "Expected '(' after 'for'.");
        ASTNode initializer = Match(TokenType.SEMICOLON) ? null : VariableDeclaration();
        ASTNode condition = Match(TokenType.SEMICOLON) ? null : Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after loop condition.");
        ASTNode increment = Match(TokenType.RPAREN) ? null : Expression();
        Consume(TokenType.RPAREN, "Expected ')' after for clauses.");
        ASTNode body = Statement();

        return new ForStatement(initializer, condition, increment, body);
    }

    private ASTNode ReturnStatement()
    {
        Token keyword = Previous();
        ASTNode value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after return value.");
        return new ReturnStatement(keyword, value);
    }

    private BlockStatement Block()
    {
        List<ASTNode> statements = new List<ASTNode>();

        while (!Check(TokenType.RBRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RBRACE, "Expected '}' after block.");
        return new BlockStatement(statements);
    }

    private ASTNode ExpressionStatement()
    {
        ASTNode expr = Expression();
        Consume(TokenType.SEMICOLON, "Expected ';' after expression.");
        return new ExpressionStatement(expr);
    }

    private ASTNode Expression()
    {
        return Assignment();
    }

    private ASTNode Assignment()
    {
        ASTNode expr = Or();

        if (Match(TokenType.ASSIGN))
        {
            Token equals = Previous();
            ASTNode value = Assignment();

            if (expr is VariableExpression)
            {
                Token name = ((VariableExpression)expr).Name;
                return new AssignmentExpression(name, value);
            }

            throw new ParserException($"Invalid assignment target at line {equals.Line}.");
        }

        return expr;
    }

    private ASTNode Or()
    {
        ASTNode expr = And();

        while (Match(TokenType.OR))
        {
            Token op = Previous();
            ASTNode right = And();
            expr = new LogicalExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode And()
    {
        ASTNode expr = Equality();

        while (Match(TokenType.AND))
        {
            Token op = Previous();
            ASTNode right = Equality();
            expr = new LogicalExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Equality()
    {
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
        ASTNode expr = Term();

        while (Match(TokenType.LESS_THAN, TokenType.GREATER_THAN, TokenType.LESS_EQUAL, TokenType.GREATER_EQUAL))
        {
            Token op = Previous();
            ASTNode right = Term();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode Term()
    {
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
        if (Match(TokenType.FALSE)) return new LiteralExpression(false);
        if (Match(TokenType.TRUE)) return new LiteralExpression(true);
        if (Match(TokenType.NULL)) return new LiteralExpression(null);

        if (Match(TokenType.NUMBER, TokenType.STRING, TokenType.CHARACTER))
        {
            return new LiteralExpression(Previous().Literal);
        }

        if (Match(TokenType.ID))
        {
            return new VariableExpression(Previous());
        }

        if (Match(TokenType.LPAREN))
        {
            ASTNode expr = Expression();
            Consume(TokenType.RPAREN, "Expected ')' after expression.");
            return new GroupingExpression(expr);
        }

        throw new ParserException("Expected expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
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

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.FOR:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}

public class ParserException : Exception
{
    public ParserException(string message) : base(message) { }
}
