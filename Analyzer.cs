using System;
using System.Collections.Generic;

public class SemanticAnalyzer : IVisitor<object?>
{
    private OptimizedAST optimizedAST;
    private Scope currentScope = new Scope();
    public SemanticAnalyzer(OptimizedAST optimizedAST)
    {
        this.optimizedAST = optimizedAST;
    }

    private Type? currentReturnType = null;

    public object? VisitBinaryExpression(BinaryExpression expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        if (left == null || right == null)
        {
            throw new Exception("Error: Operando en expresión binaria no puede ser nulo.");
        }

        if (left.GetType() != right.GetType())
        {
            throw new Exception($"Error: Los tipos de los operandos en la expresión binaria no coinciden.");
        }

        switch (expr.Operator.Type)
        {
            case TokenType.PLUS:
                if (left is int && right is int) return (int)left + (int)right;
                if (left is float && right is float) return (float)left + (float)right;
                if (left is double && right is double) return (double)left + (double)right;
                break;
            case TokenType.MINUS:
                if (left is int && right is int) return (int)left - (int)right;
                if (left is float && right is float) return (float)left - (float)right;
                if (left is double && right is double) return (double)left - (double)right;
                break;
            case TokenType.TIMES:
                if (left is int && right is int) return (int)left * (int)right;
                if (left is float && right is float) return (float)left * (float)right;
                if (left is double && right is double) return (double)left * (double)right;
                break;
            case TokenType.DIVIDE:
                if (left is int && right is int)
                {
                    if ((int)right == 0) throw new Exception("Error: División por cero.");
                    return (int)left / (int)right;
                }
                if (left is float && right is float)
                {
                    if ((float)right == 0) throw new Exception("Error: División por cero.");
                    return (float)left / (float)right;
                }
                if (left is double && right is double)
                {
                    if ((double)right == 0) throw new Exception("Error: División por cero.");
                    return (double)left / (double)right;
                }
                break;
            case TokenType.MODULO:
                if (left is int && right is int) return (int)left % (int)right;
                if (left is float && right is float) return (float)left % (float)right;
                if (left is double && right is double) return (double)left % (double)right;
                break;
        }

        throw new Exception($"Error: Operandos no compatibles para la operación '{expr.Operator.Lexeme}'");
    }

    public object? VisitUnaryExpression(UnaryExpression expr)
    {
        var operand = expr.Right.Accept(this);
        if (operand == null)
        {
            throw new Exception("Error: Operando en expresión unaria no puede ser nulo.");
        }
        return operand;
    }

    public object? VisitLiteralExpression(LiteralExpression expr)
    {
        return expr.Value;
    }

    public object? VisitGroupingExpression(GroupingExpression expr)
    {
        return expr.Expression.Accept(this);
    }

    public object? VisitVariableExpression(VariableExpression expr)
    {
        if (!currentScope.IsDefined(expr.Name.Lexeme))
        {
            throw new Exception($"Error: Variable '{expr.Name.Lexeme}' no está definida.");
        }
        return currentScope.Get(expr.Name.Lexeme);
    }

    public object? VisitAssignmentExpression(AssignmentExpression expr)
    {
        var value = expr.Value.Accept(this);
        if (!currentScope.IsDefined(expr.Name.Lexeme))
        {
            throw new Exception($"Error: Variable '{expr.Name.Lexeme}' no está definida.");
        }
        currentScope.Assign(expr.Name.Lexeme, value ?? throw new Exception("Error: Asignación de un valor nulo."));
        return value;
    }

    public object? VisitCallExpression(CallExpression expr)
    {
        var callee = expr.Callee.Accept(this);

        if (callee is FunctionDeclaration function)
        {
            var optimizedFunction = optimizedAST.GetOptimizedFunction(function.Name.Lexeme);
            if (optimizedFunction != null)
            {
                return ExecuteFunction(optimizedFunction, expr.Arguments);
            }
            return ExecuteFunction(new FunctionSubtree(function), expr.Arguments);
        }

        throw new Exception($"Error: Llamada a una función no válida o no definida.");
    }

    private object? ExecuteFunction(FunctionSubtree functionSubtree, List<ASTNode> arguments)
    {
        FunctionDeclaration function = functionSubtree.Function;

        // Crear un nuevo scope para la función
        var functionScope = currentScope.CreateChildScope();
        currentScope = functionScope;

        // Mapear argumentos a los parámetros de la función
        if (function.Parameters.Count != arguments.Count)
        {
            throw new Exception($"Error: La cantidad de argumentos no coincide con la cantidad de parámetros de la función {function.Name.Lexeme}.");
        }

        for (int i = 0; i < function.Parameters.Count; i++)
        {
            var paramName = function.Parameters[i].Name.Lexeme;
            var argumentValue = arguments[i].Accept(this);
            currentScope.Define(paramName, argumentValue);
        }

        // Ejecutar el cuerpo de la función
        object? result = null;

        foreach (var statement in function.Body.Statements)
        {
            result = statement.Accept(this);

            // Si se encuentra un valor de retorno, romper el ciclo
            if (result is ReturnStatement returnStmt)
            {
                result = returnStmt.Value.Accept(this);
                break;
            }
        }

        // Volver al scope anterior
        currentScope = currentScope.Parent;

        return result;
    }



    public object? VisitFunctionDeclaration(FunctionDeclaration stmt)
    {
        var functionScope = currentScope.CreateChildScope();
        currentScope = functionScope;

        foreach (var param in stmt.Parameters)
        {
            currentScope.Define(param.Name.Lexeme, null);
        }

        stmt.Body.Accept(this);

        optimizedAST.AnalyzeAndOptimize(new List<ASTNode> { stmt });

        currentScope = currentScope.Parent;

        return null;
    }

    public object? VisitVariableDeclaration(VariableDeclaration stmt)
    {
        var initializer = stmt.Initializer?.Accept(this);
        currentScope.Define(stmt.Name.Lexeme, initializer ?? throw new Exception("Error: Inicialización de un valor nulo."));
        return null;
    }

    public object? VisitExpressionStatement(ExpressionStatement stmt)
    {
        return stmt.Expression.Accept(this);
    }

    public object? VisitIfStatement(IfStatement stmt)
    {
        var condition = stmt.Condition.Accept(this);

        if (condition is not bool)
        {
            throw new Exception("Error: La condición de la declaración 'if' debe ser booleana.");
        }

        stmt.ThenBranch.Accept(this);
        stmt.ElseBranch?.Accept(this);
        return null;
    }

    public object? VisitWhileStatement(WhileStatement stmt)
    {
        object? conditionResult = stmt.Condition.Accept(this);
        if (conditionResult is not bool conditionValue)
        {
            throw new Exception("Error: La condición de la declaración 'while' debe ser booleana.");
        }

        while (conditionValue)
        {
            stmt.Body.Accept(this);
            conditionResult = stmt.Condition.Accept(this);
            if (conditionResult is not bool newConditionValue)
            {
                throw new Exception($"Error: La condición de la declaración 'while' debe ser booleana.");
            }
            conditionValue = newConditionValue;
        }
        return null;
    }
    public object? VisitForStatement(ForStatement stmt)
    {
        stmt.Initializer.Accept(this);
        object? conditionResult = stmt.Condition.Accept(this);

        if (conditionResult is not bool conditionValue)
        {
            throw new Exception("Error: La condición de la declaración 'for' debe ser booleana.");
        }

        while (conditionValue)
        {
            stmt.Body.Accept(this);
            stmt.Increment.Accept(this);
            conditionResult = stmt.Condition.Accept(this);
            if (conditionResult is not bool newConditionValue)
            {
                throw new Exception($"Error: La condición de la declaración 'for' debe ser booleana.");
            }
            conditionValue = newConditionValue;
        }
        return null;
    }

    public object? VisitBlockStatement(BlockStatement stmt)
    {
        var blockScope = currentScope.CreateChildScope();
        var previousScope = currentScope;
        currentScope = blockScope;

        foreach (var statement in stmt.Statements)
        {
            statement.Accept(this);
        }

        currentScope = previousScope;
        return null;
    }

    public object? VisitReturnStatement(ReturnStatement stmt)
    {
        if (currentReturnType == null)
        {
            throw new Exception("Error: La instrucción 'return' solo puede usarse dentro de una función.");
        }

        var returnValue = stmt.Value?.Accept(this);
        if (returnValue != null && returnValue.GetType() != currentReturnType)
        {
            throw new Exception($"Error: Tipo de retorno no coincide con el esperado.");
        }

        return returnValue;
    }

    public object? VisitArrayDeclaration(ArrayDeclaration stmt)
    {
        var size = stmt.Size.Accept(this);
        if (size == null)
        {
            throw new Exception("Error: El tamaño del arreglo no puede ser nulo.");
        }
        currentScope.Define(stmt.Name.Lexeme, stmt);
        return null;
    }

    public object? VisitArrayAccess(ArrayAccess expr)
    {
        var array = expr.Array.Accept(this);
        var index = expr.Index.Accept(this);
        if (array == null)
        {
            throw new Exception("Error: El arreglo accedido no puede ser nulo.");
        }
        if (index == null)
        {
            throw new Exception("Error: El índice del arreglo no puede ser nulo.");
        }
        return null;
    }

    public object? VisitArrayAssignmentExpression(ArrayAssignmentExpression expr)
    {
        var array = expr.Array.Accept(this);
        var index = expr.Index.Accept(this);
        var value = expr.Value.Accept(this);
        if (array == null || index == null || value == null)
        {
            throw new Exception("Error: Acceso o asignación en el arreglo tiene valores nulos.");
        }
        return value;
    }

    public object? VisitStructDeclaration(StructDeclaration stmt)
    {
        currentScope.Define(stmt.Name.Lexeme, stmt);
        return null;
    }

    public object? VisitEnumDeclaration(EnumDeclaration stmt)
    {
        currentScope.Define(stmt.Name.Lexeme, stmt);
        return null;
    }

    public object? VisitTernaryExpression(TernaryExpression expr)
    {
        var condition = expr.Condition.Accept(this);
        if (condition == null || condition.GetType() != typeof(bool))
        {
            throw new Exception("Error: La condición en la expresión ternaria no puede ser nula o no booleana.");
        }
        var trueExpr = expr.TrueExpr.Accept(this);
        var falseExpr = expr.FalseExpr.Accept(this);
        return (bool)condition ? trueExpr : falseExpr;
    }

    public object? VisitLambdaExpression(LambdaExpression expr)
    {
        currentScope.Define("lambda", expr);
        return expr;
    }

    public object? VisitImportStatement(ImportStatement stmt)
    {
        throw new NotImplementedException("Las declaraciones de importación no están implementadas en el analizador semántico.");
    }

    public object? VisitTryCatchStatement(TryCatchStatement stmt)
    {
        stmt.TryBlock.Accept(this);
        stmt.CatchBlock.Accept(this);
        stmt.FinallyBlock?.Accept(this);
        return null;
    }

    public object? VisitThrowStatement(ThrowStatement stmt)
    {
        stmt.Expression.Accept(this);
        return null;
    }

    public object? VisitLogicalExpression(LogicalExpression expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);
        if (left == null || right == null)
        {
            throw new Exception("Error: Operando en expresión lógica no puede ser nulo.");
        }
        return null;
    }

    public object? VisitSwitchStatement(SwitchStatement stmt)
    {
        stmt.Expression.Accept(this);
        foreach (var caseStmt in stmt.Cases)
        {
            caseStmt.Accept(this);
        }
        stmt.DefaultCase?.Accept(this);
        return null;
    }

    public object? VisitCaseStatement(CaseStatement stmt)
    {
        stmt.Value.Accept(this);
        stmt.Body.Accept(this);
        return null;
    }

    public object? VisitBreakStatement(BreakStatement stmt)
    {
        return null;
    }

    public object? VisitContinueStatement(ContinueStatement stmt)
    {
        return null;
    }
}
