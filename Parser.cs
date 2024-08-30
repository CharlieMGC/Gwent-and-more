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
        List<ASTNode> statements = new List<ASTNode>();
        while (!IsAtEnd())
        {
            try
            {
                statements.Add(Declaration());
            }
            catch (ParserException e)
            {
                Console.WriteLine(e.Message);
                Synchronize();
            }
        }
        return statements;
    }

    private ASTNode Declaration()
    {
        try
        {
            if (Match(TokenType.TYPE))
            {
                return VariableDeclaration();
            }
            if (Match(TokenType.FUN))
            {
                return FunctionDeclaration();
            }
            return Statement();
        }
        catch (ParserException error)
        {
            Synchronize();
            return null;
        }
    }

    private ASTNode VariableDeclaration()
    {
        Token type = Previous();
        Token name = Consume(TokenType.ID, "Expect variable name.");

        ASTNode initializer = null;
        if (Match(TokenType.ASSIGN))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new VariableDeclaration(type, name, initializer);
    }

    private ASTNode FunctionDeclaration()
    {
        Token name = Consume(TokenType.ID, "Expect function name.");
        Consume(TokenType.LPAREN, "Expect '(' after function name.");
        List<VariableDeclaration> parameters = new List<VariableDeclaration>();
        if (!Check(TokenType.RPAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }

                Token paramType = Consume(TokenType.TYPE, "Expect parameter type.");
                Token paramName = Consume(TokenType.ID, "Expect parameter name.");
                parameters.Add(new VariableDeclaration(paramType, paramName, null));
            } while (Match(TokenType.COMMA));
        }
        Consume(TokenType.RPAREN, "Expect ')' after parameters.");

        Consume(TokenType.LBRACE, "Expect '{' before function body.");
        BlockStatement body = (BlockStatement)Block();
        return new FunctionDeclaration(name, parameters, body);
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
        Consume(TokenType.LPAREN, "Expect '(' after 'if'.");
        ASTNode condition = Expression();
        Consume(TokenType.RPAREN, "Expect ')' after if condition.");

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
        Consume(TokenType.LPAREN, "Expect '(' after 'while'.");
        ASTNode condition = Expression();
        Consume(TokenType.RPAREN, "Expect ')' after while condition.");
        ASTNode body = Statement();

        return new WhileStatement(condition, body);
    }

    private ASTNode ForStatement()
    {
        Consume(TokenType.LPAREN, "Expect '(' after 'for'.");

        ASTNode initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.TYPE))
        {
            initializer = VariableDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        ASTNode condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

        ASTNode increment = null;
        if (!Check(TokenType.RPAREN))
        {
            increment = Expression();
        }
        Consume(TokenType.RPAREN, "Expect ')' after for clauses.");

        ASTNode body = Statement();

        // Desugar for loop into while loop
        if (increment != null)
        {
            body = new BlockStatement(new List<ASTNode>
            {
                body,
                new ExpressionStatement(increment)
            });
        }

        if (condition == null)
        {
            condition = new LiteralExpression(true);
        }
        body = new WhileStatement(condition, body);

        if (initializer != null)
        {
            body = new BlockStatement(new List<ASTNode>
            {
                initializer,
                body
            });
        }

        return body;
    }

    private ASTNode ReturnStatement()
    {
        Token keyword = Previous();
        ASTNode value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
        return new ReturnStatement(keyword, value);
    }

    private ASTNode Block()
    {
        List<ASTNode> statements = new List<ASTNode>();

        while (!Check(TokenType.RBRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RBRACE, "Expect '}' after block.");
        return new BlockStatement(statements);
    }

    private ASTNode ExpressionStatement()
    {
        ASTNode expr = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new ExpressionStatement(expr);
    }

    private ASTNode Expression()
    {
        return Assignment();
    }

    private ASTNode Assignment()
    {
        ASTNode expr = LogicalOr();

        if (Match(TokenType.ASSIGN))
        {
            Token equals = Previous();
            ASTNode value = Assignment();

            if (expr is VariableExpression ve)
            {
                Token name = ve.Name;
                return new AssignmentExpression(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private ASTNode LogicalOr()
    {
        ASTNode expr = LogicalAnd();

        while (Match(TokenType.OR))
        {
            Token op = Previous();
            ASTNode right = LogicalAnd();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private ASTNode LogicalAnd()
    {
        ASTNode expr = Equality();

        while (Match(TokenType.AND))
        {
            Token op = Previous();
            ASTNode right = Equality();
            expr = new BinaryExpression(expr, op, right);
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

        while (Match(TokenType.TIMES, TokenType.DIVIDE, TokenType.MODULO))
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

        return Call();
    }

    private ASTNode Call()
    {
        ASTNode expr = Primary();

        while (true)
        {
            if (Match(TokenType.LPAREN))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private ASTNode FinishCall(ASTNode callee)
    {
        List<ASTNode> arguments = new List<ASTNode>();
        if (!Check(TokenType.RPAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        Token paren = Consume(TokenType.RPAREN, "Expect ')' after arguments.");

        return new CallExpression(callee, paren, arguments);
    }

    private ASTNode Primary()
    {
        if (Match(TokenType.FALSE)) return new LiteralExpression(false);
        if (Match(TokenType.TRUE)) return new LiteralExpression(true);
        if (Match(TokenType.NULL)) return new LiteralExpression(null);

        if (Match(TokenType.NUMBER, TokenType.STRING))
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
            Consume(TokenType.RPAREN, "Expect ')' after expression.");
            return new GroupingExpression(expr);
        }

        throw Error(Peek(), "Expect expression.");
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
        throw Error(Peek(), message);
    }

    private ParserException Error(Token token, string message)
    {
        Console.WriteLine($"[line {token.Line}, column {token.Column}] Error at '{token.Lexeme}': {message}");
        return new ParserException(message);
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.TYPE:
                case TokenType.FUN:
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