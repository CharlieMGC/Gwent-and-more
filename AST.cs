using System.Collections.Generic;

public abstract class ASTNode
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public interface IVisitor<T>
{
    T VisitBinaryExpression(BinaryExpression expr);
    T VisitUnaryExpression(UnaryExpression expr);
    T VisitLiteralExpression(LiteralExpression expr);
    T VisitGroupingExpression(GroupingExpression expr);
    T VisitVariableExpression(VariableExpression expr);
    T VisitAssignmentExpression(AssignmentExpression expr);
    T VisitCallExpression(CallExpression expr);
    T VisitVariableDeclaration(VariableDeclaration stmt);
    T VisitFunctionDeclaration(FunctionDeclaration stmt);
    T VisitExpressionStatement(ExpressionStatement stmt);
    T VisitIfStatement(IfStatement stmt);
    T VisitWhileStatement(WhileStatement stmt);
    T VisitForStatement(ForStatement stmt);
    T VisitBlockStatement(BlockStatement stmt);
    T VisitReturnStatement(ReturnStatement stmt);
}

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

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
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

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }
}

public class LiteralExpression : ASTNode
{
    public object Value { get; }

    public LiteralExpression(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}

public class GroupingExpression : ASTNode
{
    public ASTNode Expression { get; }

    public GroupingExpression(ASTNode expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}

public class VariableExpression : ASTNode
{
    public Token Name { get; }

    public VariableExpression(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableExpression(this);
    }
}

public class AssignmentExpression : ASTNode
{
    public Token Name { get; }
    public ASTNode Value { get; }

    public AssignmentExpression(Token name, ASTNode value)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitAssignmentExpression(this);
    }
}

public class CallExpression : ASTNode
{
    public ASTNode Callee { get; }
    public Token Paren { get; }
    public List<ASTNode> Arguments { get; }

    public CallExpression(ASTNode callee, Token paren, List<ASTNode> arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitCallExpression(this);
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

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableDeclaration(this);
    }
}

public class FunctionDeclaration : ASTNode
{
    public Token Name { get; }
    public List<VariableDeclaration> Parameters { get; }
    public BlockStatement Body { get; }

    public FunctionDeclaration(Token name, List<VariableDeclaration> parameters, BlockStatement body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitFunctionDeclaration(this);
    }
}

public class ExpressionStatement : ASTNode
{
    public ASTNode Expression { get; }

    public ExpressionStatement(ASTNode expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitExpressionStatement(this);
    }
}

public class IfStatement : ASTNode
{
    public ASTNode Condition { get; }
    public ASTNode ThenBranch { get; }
    public ASTNode ElseBranch { get; }

    public IfStatement(ASTNode condition, ASTNode thenBranch, ASTNode elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIfStatement(this);
    }
}

public class WhileStatement : ASTNode
{
    public ASTNode Condition { get; }
    public ASTNode Body { get; }

    public WhileStatement(ASTNode condition, ASTNode body)
    {
        Condition = condition;
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitWhileStatement(this);
    }
}

public class ForStatement : ASTNode
{
    public ASTNode Initializer { get; }
    public ASTNode Condition { get; }
    public ASTNode Increment { get; }
    public ASTNode Body { get; }

    public ForStatement(ASTNode initializer, ASTNode condition, ASTNode increment, ASTNode body)
    {
        Initializer = initializer;
        Condition = condition;
        Increment = increment;
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitForStatement(this);
    }
}

public class BlockStatement : ASTNode
{
    public List<ASTNode> Statements { get; }

    public BlockStatement(List<ASTNode> statements)
    {
        Statements = statements;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBlockStatement(this);
    }
}

public class ReturnStatement : ASTNode
{
    public Token Keyword { get; }
    public ASTNode Value { get; }

    public ReturnStatement(Token keyword, ASTNode value)
    {
        Keyword = keyword;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitReturnStatement(this);
    }
}