using System;
using System.Collections.Generic;

public class SemanticAnalyzer : IVisitor<object?>
{
    private Scope currentScope = new Scope(); // Inicialización del scope actual
    private Type? currentReturnType = null; // Para manejar el tipo de retorno de la función actual

    public object? VisitBinaryExpression(BinaryExpression expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        if (left == null || right == null)
        {
            throw new Exception("Error: Operando en expresión binaria no puede ser nulo.");
        }

        // Validación de que ambos operandos sean del mismo tipo
        if (left.GetType() != right.GetType())
        {
            throw new Exception($"Error: Los tipos de los operandos en la expresión binaria no coinciden. Left: {left.GetType()}, Right: {right.GetType()}");
        }

        Console.WriteLine($"Comparando operandos: left = {left} ({left.GetType()}), right = {right} ({right.GetType()})");

        // Operaciones aritméticas
        switch (expr.Operator.Type)
        {
            case TokenType.PLUS:
                if (left is int && right is int)
                    return (int)left + (int)right;
                if (left is float && right is float)
                    return (float)left + (float)right;
                if (left is double && right is double)
                    return (double)left + (double)right;
                break;
            case TokenType.MINUS:
                if (left is int && right is int)
                    return (int)left - (int)right;
                if (left is float && right is float)
                    return (float)left - (float)right;
                if (left is double && right is double)
                    return (double)left - (double)right;
                break;
            case TokenType.TIMES:
                if (left is int && right is int)
                    return (int)left * (int)right;
                if (left is float && right is float)
                    return (float)left * (float)right;
                if (left is double && right is double)
                    return (double)left * (double)right;
                break;
            case TokenType.DIVIDE:
                if (left is int && right is int)
                {
                    if ((int)right == 0)
                        throw new Exception("Error: División por cero.");
                    return (int)left / (int)right;
                }
                if (left is float && right is float)
                {
                    if ((float)right == 0)
                        throw new Exception("Error: División por cero.");
                    return (float)left / (float)right;
                }
                if (left is double && right is double)
                {
                    if ((double)right == 0)
                        throw new Exception("Error: División por cero.");
                    return (double)left / (double)right;
                }
                break;
            case TokenType.MODULO:
                if (left is int && right is int)
                    return (int)left % (int)right;
                if (left is float && right is float)
                    return (float)left % (float)right;
                if (left is double && right is double)
                    return (double)left % (double)right;
                break;

            // Operaciones lógicas
            case TokenType.AND:
            case TokenType.OR:
                if (left is bool && right is bool)
                {
                    return expr.Operator.Type == TokenType.AND ? (bool)left && (bool)right : (bool)left || (bool)right;
                }
                break;

            // Operaciones de comparación
            case TokenType.LESS_THAN:
            case TokenType.LESS_EQUAL:
            case TokenType.GREATER_THAN:
            case TokenType.GREATER_EQUAL:
                if (left is int && right is int)
                    return expr.Operator.Type switch
                    {
                        TokenType.LESS_THAN => (int)left < (int)right,
                        TokenType.LESS_EQUAL => (int)left <= (int)right,
                        TokenType.GREATER_THAN => (int)left > (int)right,
                        TokenType.GREATER_EQUAL => (int)left >= (int)right,
                        _ => throw new Exception("Operador no válido para comparación.")
                    };
                if (left is float && right is float)
                    return expr.Operator.Type switch
                    {
                        TokenType.LESS_THAN => (float)left < (float)right,
                        TokenType.LESS_EQUAL => (float)left <= (float)right,
                        TokenType.GREATER_THAN => (float)left > (float)right,
                        TokenType.GREATER_EQUAL => (float)left >= (float)right,
                        _ => throw new Exception("Operador no válido para comparación.")
                    };
                if (left is double && right is double)
                    return expr.Operator.Type switch
                    {
                        TokenType.LESS_THAN => (double)left < (double)right,
                        TokenType.LESS_EQUAL => (double)left <= (double)right,
                        TokenType.GREATER_THAN => (double)left > (double)right,
                        TokenType.GREATER_EQUAL => (double)left >= (double)right,
                        _ => throw new Exception("Operador no válido para comparación.")
                    };
                break;

            // Operaciones de igualdad
            case TokenType.EQUAL:
            case TokenType.NOT_EQUAL:
                return expr.Operator.Type == TokenType.EQUAL ? Equals(left, right) : !Equals(left, right);

            default:
                throw new Exception($"Error: Operador '{expr.Operator.Lexeme}' no soportado en esta operación.");
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
        if (callee == null || !(callee is FunctionDeclaration function))
        {
            throw new Exception($"Error: Llamada a una función no definida o inválida.");
        }

        // Verificar que la cantidad de argumentos coincida
        if (function.Parameters.Count != expr.Arguments.Count)
        {
            throw new Exception("Error: Número incorrecto de argumentos en la llamada a la función.");
        }

        // Verificar que los tipos de los argumentos coincidan con los tipos de los parámetros
        for (int i = 0; i < expr.Arguments.Count; i++)
        {
            var argumentValue = expr.Arguments[i].Accept(this);
            var parameterType = function.Parameters[i].Type.Lexeme;

            if (argumentValue == null || argumentValue.GetType().Name != parameterType)
            {
                throw new Exception($"Error: Tipo del argumento en la posición {i + 1} no coincide con el tipo del parámetro esperado ({parameterType}).");
            }
        }

        // Ejecutar el cuerpo de la función en un nuevo ámbito
        var previousReturnType = currentReturnType;
        currentReturnType = Type.GetType(function.Body.GetType().Name);

        // Crear un nuevo scope para la función
        var functionScope = currentScope.CreateChildScope();
        var previousScope = currentScope;
        currentScope = functionScope;

        function.Body.Accept(this);

        // Restaurar el scope anterior
        currentScope = previousScope;
        currentReturnType = previousReturnType;

        return null; // Suponiendo que no hay un valor de retorno explícito por ahora.
    }

    public object? VisitVariableDeclaration(VariableDeclaration stmt)
    {
        var initializer = stmt.Initializer.Accept(this);
        currentScope.Define(stmt.Name.Lexeme, initializer ?? throw new Exception("Error: Inicialización de un valor nulo."));
        return null;
    }

    public object? VisitFunctionDeclaration(FunctionDeclaration stmt)
    {
        currentScope.Define(stmt.Name.Lexeme, stmt);
        return null;
    }

    public object? VisitExpressionStatement(ExpressionStatement stmt)
    {
        return stmt.Expression.Accept(this);
    }

    public object? VisitIfStatement(IfStatement stmt)
    {
        var condition = stmt.Condition.Accept(this);
        Console.WriteLine($"Evaluando condición del 'if': {condition} (Tipo: {condition?.GetType()?.Name})");

        if (condition == null || condition.GetType() != typeof(bool))
        {
            throw new Exception("Error: La condición de la declaración 'if' no puede ser nula o no booleana.");
        }

        stmt.ThenBranch.Accept(this);
        stmt.ElseBranch?.Accept(this);
        return null;
    }

    public object? VisitWhileStatement(WhileStatement stmt)
    {
        var condition = stmt.Condition.Accept(this);
        if (condition == null || condition.GetType() != typeof(bool))
        {
            throw new Exception("Error: La condición de la declaración 'while' no puede ser nula o no booleana.");
        }

        Console.WriteLine($"Evaluando condición del 'while': {condition} (Tipo: {condition.GetType().Name})");

        while ((bool)condition)
        {
            stmt.Body.Accept(this);
            condition = stmt.Condition.Accept(this); // Re-evaluar la condición después de cada iteración
            Console.WriteLine($"Evaluando nuevamente la condición del 'while': {condition} (Tipo: {condition.GetType().Name})");
        }

        Console.WriteLine("El 'while' ha terminado.");
        return null;
    }
    public object? VisitForStatement(ForStatement stmt)
    {
        Console.WriteLine("Iniciando ejecución del 'for'.");

        // Crear un nuevo scope para el 'for'
        var forScope = currentScope.CreateChildScope();
        var previousScope = currentScope;
        currentScope = forScope;

        // Inicialización del 'for'
        stmt.Initializer?.Accept(this);

        var condition = stmt.Condition?.Accept(this);
        Console.WriteLine($"Condición inicial del 'for': {condition} (Tipo: {condition?.GetType()?.Name})");

        // Ejecutar el cuerpo del 'for' mientras la condición sea verdadera
        while (condition != null && (bool)condition)
        {
            Console.WriteLine("Ejecutando cuerpo del 'for'");
            stmt.Body.Accept(this); // Ejecutar el cuerpo del 'for'
            stmt.Increment?.Accept(this); // Ejecutar la expresión de incremento
            condition = stmt.Condition.Accept(this); // Re-evaluar la condición después de cada iteración
            Console.WriteLine($"Evaluando nuevamente la condición del 'for': {condition} (Tipo: {condition?.GetType()?.Name})");
        }

        // Restaurar el scope anterior
        currentScope = previousScope;
        Console.WriteLine("Finalizando ejecución del 'for'.");

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
        var returnValue = stmt.Value.Accept(this);
        Console.WriteLine($"Return encontrado con valor: {returnValue} (Tipo: {returnValue?.GetType()?.Name})");

        if (currentReturnType != null)
        {
            if (returnValue != null && returnValue.GetType() != currentReturnType)
            {
                throw new Exception($"Error: Tipo de retorno no coincide. Se esperaba '{currentReturnType}', pero se obtuvo '{returnValue.GetType()}'.");
            }
        }

        return returnValue; // Devuelve el valor de retorno
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
        return condition != null ? trueExpr : falseExpr;
    }

    public object? VisitLambdaExpression(LambdaExpression expr)
    {
        // Las expresiones lambda se tratan como funciones anónimas.
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
