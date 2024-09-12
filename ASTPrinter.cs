using System;
using System.Collections.Generic;
using System.Text;

public class ASTPrinter : IVisitor<string>
{
    public string VisitProgramNode(ProgramNode program)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Program:");

        builder.AppendLine("Functions:");
        if (program.FunctionDeclarations.Count == 0)
        {
            builder.AppendLine("No functions found."); // Depuración
        }

        foreach (var function in program.FunctionDeclarations)
        {
            Console.WriteLine($"Imprimiendo función: {function.Name.Lexeme}"); // Depuración
            builder.AppendLine(function.Accept(this));  // Llama a VisitFunctionDeclaration
        }
        // Imprimir las declaraciones restantes
        builder.AppendLine("Statements:");
        foreach (var statement in program.Statements)
        {
            builder.AppendLine(statement.Accept(this));
        }

        return builder.ToString();
    }

    public string VisitBinaryExpression(BinaryExpression expr)
    {
        string left = expr.Left.Accept(this);
        string right = expr.Right.Accept(this);
        return $"({left} {expr.Operator.Lexeme} {right})";
    }

    public string VisitUnaryExpression(UnaryExpression expr)
    {
        string right = expr.Right.Accept(this);
        return $"({expr.Operator.Lexeme}{right})";
    }

    public string VisitLiteralExpression(LiteralExpression expr)
    {
        return expr.Value?.ToString() ?? "null";
    }

    public string VisitGroupingExpression(GroupingExpression expr)
    {
        string expression = expr.Expression.Accept(this);
        Console.WriteLine($"Printing grouping expression: {expression}");
        return $"({expression})";
    }

    public string VisitVariableExpression(VariableExpression expr)
    {
        return expr.Name.Lexeme;
    }

    public string VisitAssignmentExpression(AssignmentExpression expr)
    {
        string name = expr.Name.Lexeme;
        string value = expr.Value.Accept(this);
        return $"{name} = {value}";
    }

    public string VisitCallExpression(CallExpression expr)
    {
        string callee = expr.Callee.Accept(this);
        string arguments = string.Join(", ", expr.Arguments.ConvertAll(arg => arg.Accept(this)));
        return $"{callee}({arguments})";
    }

    public string VisitArrayAccess(ArrayAccess expr)
    {
        string array = expr.Array.Accept(this);
        string index = expr.Index.Accept(this);
        return $"{array}[{index}]";
    }

    public string VisitArrayAssignmentExpression(ArrayAssignmentExpression expr)
    {
        string arrayAccess = expr.Array.Accept(this);
        string index = expr.Index.Accept(this);
        string value = expr.Value.Accept(this);
        return $"{arrayAccess}[{index}] = {value}";
    }

    public string VisitLogicalExpression(LogicalExpression expr)
    {
        string left = expr.Left.Accept(this);
        string right = expr.Right.Accept(this);
        return $"({left} {expr.Operator.Lexeme} {right})";
    }

    public string VisitTernaryExpression(TernaryExpression expr)
    {
        string condition = expr.Condition.Accept(this);
        string trueExpr = expr.TrueExpr.Accept(this);
        string falseExpr = expr.FalseExpr.Accept(this);
        return $"({condition} ? {trueExpr} : {falseExpr})";
    }

    public string VisitLambdaExpression(LambdaExpression expr)
    {
        string parameters = string.Join(", ", expr.Parameters.ConvertAll(param => param.Accept(this)));
        string body = expr.Body.Accept(this);
        return $"({parameters}) => {body}";
    }

    public string VisitThrowStatement(ThrowStatement stmt)
    {
        string expression = stmt.Expression.Accept(this);
        return $"throw {expression};";
    }

    public string VisitVariableDeclaration(VariableDeclaration stmt)
    {
        string type = stmt.Type.Lexeme;
        string name = stmt.Name.Lexeme;
        string? initializer = stmt.Initializer?.Accept(this);
        return $"{type} {name} = {initializer};";
    }

    public string VisitFunctionDeclaration(FunctionDeclaration stmt)
    {
        string name = stmt.Name.Lexeme;
        string parameters = string.Join(", ", stmt.Parameters.Select(param => $"{param.Type.Lexeme} {param.Name.Lexeme}"));
        string body = stmt.Body.Accept(this);
        return $"function {name}({parameters}) {body}";
    }

    public string VisitExpressionStatement(ExpressionStatement stmt)
    {
        string expression = stmt.Expression.Accept(this);
        return $"{expression};";
    }

    public string VisitIfStatement(IfStatement stmt)
    {
        string condition = stmt.Condition.Accept(this);
        string thenBranch = stmt.ThenBranch.Accept(this);
        string elseBranch = stmt.ElseBranch != null ? $" else {stmt.ElseBranch.Accept(this)}" : string.Empty;
        return $"if ({condition}) {thenBranch}{elseBranch}";
    }

    public string VisitWhileStatement(WhileStatement stmt)
    {
        string condition = stmt.Condition.Accept(this);
        string body = stmt.Body.Accept(this);
        return $"while ({condition}) {body}";
    }

    public string VisitForStatement(ForStatement stmt)
    {
        string initializer = stmt.Initializer.Accept(this);
        string condition = stmt.Condition.Accept(this);
        string increment = stmt.Increment.Accept(this);
        string body = stmt.Body.Accept(this);
        return $"for ({initializer}; {condition}; {increment}) {body}";
    }

    public string VisitBlockStatement(BlockStatement stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("{");
        foreach (var statement in stmt.Statements)
        {
            string stmtString = statement.Accept(this);
            builder.AppendLine($"  {stmtString}");
        }
        builder.Append("}");
        return builder.ToString();
    }

    public string VisitReturnStatement(ReturnStatement stmt)
    {
        string value = stmt.Value.Accept(this);
        return $"return {value};";
    }

    public string VisitArrayDeclaration(ArrayDeclaration stmt)
    {
        string type = stmt.Type.Lexeme;
        string name = stmt.Name.Lexeme;
        string size = stmt.Size.Accept(this);
        string initializer = stmt.Initializer.Count > 0
            ? $" = {{ {string.Join(", ", stmt.Initializer.ConvertAll(init => init.Accept(this)))} }}"
            : string.Empty;
        return $"{type} {name}[{size}]{initializer};";
    }

    public string VisitStructDeclaration(StructDeclaration stmt)
    {
        string name = stmt.Name.Lexeme;
        string fields = string.Join("\n  ", stmt.Fields.ConvertAll(field => field.Accept(this)));
        return $"struct {name} {{\n  {fields}\n}}";
    }

    public string VisitEnumDeclaration(EnumDeclaration stmt)
    {
        string name = stmt.Name.Lexeme;
        string values = string.Join(", ", stmt.Values.ConvertAll(value => value.Lexeme));
        return $"enum {name} {{ {values} }}";
    }

    public string VisitImportStatement(ImportStatement stmt)
    {
        return $"import {stmt.Module.Lexeme};";
    }

    public string VisitTryCatchStatement(TryCatchStatement stmt)
    {
        string tryBlock = stmt.TryBlock.Accept(this);
        string catchParam = stmt.CatchParameter.Accept(this);
        string catchBlock = stmt.CatchBlock.Accept(this);
        string finallyBlock = stmt.FinallyBlock != null ? $" finally {stmt.FinallyBlock.Accept(this)}" : string.Empty;
        return $"try {tryBlock} catch ({catchParam}) {catchBlock}{finallyBlock}";
    }

    public string VisitSwitchStatement(SwitchStatement stmt)
    {
        string expression = stmt.Expression.Accept(this);
        string cases = string.Join("\n", stmt.Cases.ConvertAll(caseStmt => caseStmt.Accept(this)));
        string defaultCase = stmt.DefaultCase != null ? $"default: {stmt.DefaultCase.Accept(this)}" : string.Empty;
        return $"switch ({expression}) {{\n{cases}\n{defaultCase}\n}}";
    }

    public string VisitCaseStatement(CaseStatement stmt)
    {
        string value = stmt.Value.Accept(this);
        string body = stmt.Body.Accept(this);
        return $"case {value}: {body}";
    }

    public string VisitBreakStatement(BreakStatement stmt)
    {
        return "break;";
    }

    public string VisitContinueStatement(ContinueStatement stmt)
    {
        return "continue;";
    }
}
