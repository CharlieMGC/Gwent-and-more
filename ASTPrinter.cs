    using System;
    using System.Text;
    using System.Collections.Generic;

    public class ASTPrinter : IVisitor<string>
    {
        private OptimizedAST optimizedAST;

        public ASTPrinter(OptimizedAST optimizedAST)
        {
            this.optimizedAST = optimizedAST;
        }

        public string Print(ASTNode node)
        {
            return node.Accept(this);
        }

        public string VisitBinaryExpression(BinaryExpression expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitUnaryExpression(UnaryExpression expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitLiteralExpression(LiteralExpression expr)
        {
            return expr.Value?.ToString() ?? "null";
        }

        public string VisitGroupingExpression(GroupingExpression expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitVariableExpression(VariableExpression expr)
        {
            return expr.Name.Lexeme;
        }

        public string VisitAssignmentExpression(AssignmentExpression expr)
        {
            return Parenthesize($"{expr.Name.Lexeme} =", expr.Value);
        }

        public string VisitCallExpression(CallExpression expr)
        {
            var args = string.Join(", ", expr.Arguments.ConvertAll(arg => arg.Accept(this)));
            return $"{expr.Callee.Accept(this)}({args})";
        }

        public string VisitVariableDeclaration(VariableDeclaration stmt)
        {
            var initializer = stmt.Initializer != null ? " = " + stmt.Initializer.Accept(this) : "";
            return $"{stmt.Type.Lexeme} {stmt.Name.Lexeme}{initializer};";
        }

        public string VisitFunctionDeclaration(FunctionDeclaration stmt)
        {
            var parameters = string.Join(", ", stmt.Parameters.ConvertAll(param => param.Name.Lexeme));
            var body = stmt.Body.Accept(this);

            // Si la función tiene un subnodo optimizado, lo añadimos al AST impreso
            var optimizedFunction = optimizedAST.GetOptimizedFunction(stmt.Name.Lexeme);
            if (optimizedFunction != null)
            {
                return $"fun {stmt.Name.Lexeme}({parameters}) Optimized {{ {body} }}";
            }

            return $"fun {stmt.Name.Lexeme}({parameters}) {{ {body} }}";
        }

        public string VisitExpressionStatement(ExpressionStatement stmt)
        {
            return stmt.Expression.Accept(this) + ";";
        }

        public string VisitIfStatement(IfStatement stmt)
        {
            var thenBranch = stmt.ThenBranch.Accept(this);
            var elseBranch = stmt.ElseBranch != null ? " else " + stmt.ElseBranch.Accept(this) : "";
            return $"if ({stmt.Condition.Accept(this)}) {thenBranch}{elseBranch}";
        }

        public string VisitWhileStatement(WhileStatement stmt)
        {
            return $"while ({stmt.Condition.Accept(this)}) {stmt.Body.Accept(this)}";
        }

        public string VisitForStatement(ForStatement stmt)
        {
            var initializer = stmt.Initializer.Accept(this);
            var condition = stmt.Condition.Accept(this);
            var increment = stmt.Increment.Accept(this);
            return $"for ({initializer}; {condition}; {increment}) {stmt.Body.Accept(this)}";
        }

        public string VisitBlockStatement(BlockStatement stmt)
        {
            var statements = string.Join(" ", stmt.Statements.ConvertAll(s => s.Accept(this)));
            return $"{{ {statements} }}";
        }

        public string VisitReturnStatement(ReturnStatement stmt)
        {
            var value = stmt.Value != null ? stmt.Value.Accept(this) : "";
            return $"return {value};";
        }

        public string VisitArrayDeclaration(ArrayDeclaration stmt)
        {
            var size = stmt.Size.Accept(this);
            var initializer = stmt.Initializer != null && stmt.Initializer.Count > 0
                ? " = {" + string.Join(", ", stmt.Initializer.ConvertAll(i => i.Accept(this))) + "}"
                : "";
            return $"{stmt.Type.Lexeme}[] {stmt.Name.Lexeme}[{size}]{initializer};";
        }

        public string VisitArrayAccess(ArrayAccess expr)
        {
            return $"{expr.Array.Accept(this)}[{expr.Index.Accept(this)}]";
        }

        public string VisitArrayAssignmentExpression(ArrayAssignmentExpression expr)
        {
            return $"{expr.Array.Accept(this)}[{expr.Index.Accept(this)}] = {expr.Value.Accept(this)};";
        }

        public string VisitStructDeclaration(StructDeclaration stmt)
        {
            var fields = string.Join("; ", stmt.Fields.ConvertAll(f => $"{f.Type.Lexeme} {f.Name.Lexeme}"));
            return $"struct {stmt.Name.Lexeme} {{ {fields} }}";
        }

        public string VisitEnumDeclaration(EnumDeclaration stmt)
        {
            var values = string.Join(", ", stmt.Values.ConvertAll(v => v.Lexeme));
            return $"enum {stmt.Name.Lexeme} {{ {values} }}";
        }

        public string VisitTernaryExpression(TernaryExpression expr)
        {
            return $"({expr.Condition.Accept(this)} ? {expr.TrueExpr.Accept(this)} : {expr.FalseExpr.Accept(this)})";
        }

        public string VisitLambdaExpression(LambdaExpression expr)
        {
            var parameters = string.Join(", ", expr.Parameters.ConvertAll(p => p.Name.Lexeme));
            return $"lambda ({parameters}) {expr.Body.Accept(this)}";
        }

        public string VisitImportStatement(ImportStatement stmt)
        {
            return $"import {stmt.Module.Lexeme};";
        }

        public string VisitTryCatchStatement(TryCatchStatement stmt)
        {
            var tryBlock = stmt.TryBlock.Accept(this);
            var catchBlock = stmt.CatchBlock.Accept(this);
            var finallyBlock = stmt.FinallyBlock != null ? " finally " + stmt.FinallyBlock.Accept(this) : "";
            return $"try {tryBlock} catch ({stmt.CatchParameter.Name.Lexeme}) {catchBlock}{finallyBlock}";
        }

        public string VisitThrowStatement(ThrowStatement stmt)
        {
            return $"throw {stmt.Expression.Accept(this)};";
        }

        public string VisitLogicalExpression(LogicalExpression expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitSwitchStatement(SwitchStatement stmt)
        {
            var cases = string.Join(" ", stmt.Cases.ConvertAll(c => c.Accept(this)));
            var defaultCase = stmt.DefaultCase != null ? " default: " + stmt.DefaultCase.Accept(this) : "";
            return $"switch ({stmt.Expression.Accept(this)}) {{ {cases}{defaultCase} }}";
        }

        public string VisitCaseStatement(CaseStatement stmt)
        {
            return $"case {stmt.Value.Accept(this)}: {stmt.Body.Accept(this)}";
        }

        public string VisitBreakStatement(BreakStatement stmt)
        {
            return "break;";
        }

        public string VisitContinueStatement(ContinueStatement stmt)
        {
            return "continue;";
        }

        private string Parenthesize(string name, params ASTNode[] nodes)
        {
            var builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (var node in nodes)
            {
                if (node != null)
                {
                    builder.Append(" ");
                    builder.Append(node.Accept(this));
                }
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
