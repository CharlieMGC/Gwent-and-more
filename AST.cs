// AST.cs
public abstract class ASTNode { }

public class BinaryExpression : ASTNode
{
    public ASTNode Left { get; }
    public Token Operator { get; }
    public ASTNode Right { get; }

    public BinaryExpression(ASTNode left, Token op, ASTNode right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}

public class LiteralExpression : ASTNode
{
    public Token Value { get; }

    public LiteralExpression(Token value)
    {
        Value = value;
    }
}

public class VariableExpression : ASTNode
{
    public Token Name { get; }

    public VariableExpression(Token name)
    {
        Name = name;
    }
}

public class GroupingExpression : ASTNode
{
    public ASTNode Expression { get; }

    public GroupingExpression(ASTNode expression)
    {
        Expression = expression;
    }
}

public class UnaryExpression : ASTNode
{
    public Token Operator { get; }
    public ASTNode Right { get; }

    public UnaryExpression(Token op, ASTNode right)
    {
        Operator = op;
        Right = right;
    }
}

public class VariableDeclaration : ASTNode
{
    public Token Type { get; }
    public Token Name { get; }
    public ASTNode Initializer { get; }

    public VariableDeclaration(Token type, Token name, ASTNode initializer)
    {
        Type = type;
        Name = name;
        Initializer = initializer;
    }
}
