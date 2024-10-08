using System;
using System.Collections.Generic;

public class SemanticAnalyzer : IVisitor<object?>
{
    private Scope currentScope = new Scope(); // Inicialización del scope actual
    private Type? currentReturnType = null; // Para manejar el tipo de retorno de la función actual
    private FunctionTable functionTable = new FunctionTable();  // Tabla global de funciones

    public object? VisitProgramNode(ProgramNode program)
    {
        // Primero registrar las funciones en la tabla de funciones
        foreach (var function in program.FunctionDeclarations)
        {
            function.Accept(this); // Esto llamará a VisitFunctionDeclaration para registrar las funciones
        }

        // Luego ejecutar el resto de las declaraciones
        foreach (var statement in program.Statements)
        {
            statement.Accept(this);
        }

        return null;
    }

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
        // Buscar el valor de la variable en el scope actual
        if (!currentScope.IsDefined(expr.Name.Lexeme))
        {
            throw new Exception($"Error: Variable '{expr.Name.Lexeme}' no está definida.");
        }
        return currentScope.Get(expr.Name.Lexeme);
    }

    public object? VisitAssignmentExpression(AssignmentExpression expr)
    {
        var value = expr.Value.Accept(this);  // Evaluar el valor que estamos asignando
        if (!currentScope.IsDefined(expr.Name.Lexeme))
        {
            throw new Exception($"Error: Variable '{expr.Name.Lexeme}' no está definida.");
        }
        currentScope.Assign(expr.Name.Lexeme, value ?? throw new Exception("Error: Asignación de un valor nulo."));
        return value;
    }

    public object? VisitCallExpression(CallExpression expr)
    {
        // Obtener el nombre de la función que está siendo llamada
        var calleeName = expr.Callee.Accept(this)?.ToString();

        if (calleeName == null)
        {
            throw new Exception("Error: Llamada a una función no válida. El nombre de la función no puede ser null.");
        }

        // Buscar la función en la tabla de funciones
        var function = functionTable.GetFunction(calleeName);

        // Crear un nuevo scope para la función
        var functionScope = currentScope.CreateChildScope();
        var previousScope = currentScope;  // Guardamos el scope actual para restaurarlo después
        currentScope = functionScope;      // Cambiamos al nuevo scope de la función

        // Verificar que la cantidad de argumentos coincida con la cantidad de parámetros
        if (function.Parameters.Count != expr.Arguments.Count)
        {
            throw new Exception($"Error: La función '{calleeName}' esperaba {function.Parameters.Count} argumentos, pero recibió {expr.Arguments.Count}.");
        }

        // Asignar los valores de los argumentos a los parámetros
        for (int i = 0; i < expr.Arguments.Count; i++)
        {
            var argumentValue = expr.Arguments[i].Accept(this);  // Evaluar el argumento
            var param = function.Parameters[i];
            currentScope.Define(param.Name.Lexeme, argumentValue);  // Definir el parámetro en el nuevo scope
        }

        // Ejecutar el cuerpo de la función
        object? returnValue = null;
        try
        {
            returnValue = function.Body.Accept(this);  // Ejecutar el cuerpo de la función
        }
        finally
        {
            // Restaurar el scope anterior después de ejecutar la función
            currentScope = previousScope;
        }

        return returnValue;
    }

    public object? VisitFunctionDeclaration(FunctionDeclaration stmt)
    {
        // Registrar la función en la tabla de funciones
        functionTable.DefineFunction(stmt.Name.Lexeme, stmt);

        // No ejecutamos el cuerpo de la función inmediatamente, solo la registramos
        return null;
    }

    public object? VisitVariableDeclaration(VariableDeclaration stmt)
    {
        var initializer = stmt.Initializer?.Accept(this);  // Evaluar el valor inicial de la variable
        currentScope.Define(stmt.Name.Lexeme, initializer ?? throw new Exception("Error: Inicialización de un valor nulo."));
        return null;
    }

    public object? VisitExpressionStatement(ExpressionStatement stmt)
    {
        return stmt.Expression.Accept(this);  // Evaluar la expresión
    }

    public object? VisitIfStatement(IfStatement stmt)
    {
        var condition = stmt.Condition.Accept(this);
        Console.WriteLine($"Evaluando condición del 'if': {condition} (Tipo: {condition?.GetType()?.Name})");

        if (condition == null || condition.GetType() != typeof(bool))
        {
            throw new Exception("Error: La condición de la declaración 'if' no puede ser nula o no booleana.");
        }

        if ((bool)condition)
        {
            stmt.ThenBranch.Accept(this);
        }
        else
        {
            stmt.ElseBranch?.Accept(this);
        }
        return null;
    }

    public object? VisitWhileStatement(WhileStatement stmt)
    {
        object? conditionResult = stmt.Condition.Accept(this);

        if (conditionResult == null)
        {
            throw new Exception("Error: La condición de la declaración 'while' no puede ser nula.");
        }

        if (conditionResult is not bool conditionValue)
        {
            throw new Exception($"Error: La condición de la declaración 'while' debe ser booleana, pero es de tipo {conditionResult.GetType().Name}.");
        }

        Console.WriteLine($"Evaluando condición del 'while': {conditionValue} (Tipo: {conditionValue.GetType().Name})");

        while (conditionValue)
        {
            stmt.Body.Accept(this);

            // Re-evaluar la condición después de cada iteración
            conditionResult = stmt.Condition.Accept(this);

            if (conditionResult == null)
            {
                throw new Exception("Error: La condición de la declaración 'while' se volvió nula durante la ejecución.");
            }

            if (conditionResult is not bool newConditionValue)
            {
                throw new Exception($"Error: La condición de la declaración 'while' se volvió no booleana durante la ejecución. Tipo actual: {conditionResult.GetType().Name}.");
            }

            conditionValue = newConditionValue;
        }

        Console.WriteLine("El 'while' ha terminado.");
        return null;
    }

    public object? VisitForStatement(ForStatement stmt)
    {
        // Crear un nuevo scope para el ciclo for
        var forScope = currentScope.CreateChildScope();
        var previousScope = currentScope;
        currentScope = forScope;

        // Evaluar la inicialización del ciclo
        if (stmt.Initializer != null)
        {
            stmt.Initializer.Accept(this);
        }

        // Evaluar la condición del ciclo (debe ser booleana)
        if (stmt.Condition != null)
        {
            var condition = stmt.Condition.Accept(this);
            if (condition == null || condition.GetType() != typeof(bool))
            {
                throw new Exception("Error: La condición del ciclo 'for' debe ser booleana.");
            }
        }

        // Evaluar el cuerpo del ciclo
        stmt.Body.Accept(this);

        // Evaluar la expresión de incremento
        if (stmt.Increment != null)
        {
            stmt.Increment.Accept(this);
        }

        // Restaurar el scope anterior
        currentScope = previousScope;

        return null;
    }

    public object? VisitBlockStatement(BlockStatement stmt)
    {
        var blockScope = currentScope.CreateChildScope();  // Crear un nuevo scope para el bloque
        var previousScope = currentScope;  // Guardamos el scope actual
        currentScope = blockScope;  // Cambiamos al nuevo scope

        foreach (var statement in stmt.Statements)
        {
            statement.Accept(this);  // Ejecutar cada declaración en el bloque
        }

        currentScope = previousScope;  // Restaurar el scope anterior
        return null;
    }

    public object? VisitReturnStatement(ReturnStatement stmt)
    {
        var returnValue = stmt.Value.Accept(this);  // Evaluar el valor que se está retornando
        return returnValue;  // Devolver el valor a la función que lo llamó
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
