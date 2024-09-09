using System;
using System.Collections.Generic;

public class Scope
{
    private Dictionary<string, object?> variables = new Dictionary<string, object?>();
    public Scope? Parent { get; }

    private OptimizedAST optimizedAST;

    public Scope(Scope? parent = null, OptimizedAST? optimizedAST = null)
    {
        Parent = parent;
        this.optimizedAST = optimizedAST ?? new OptimizedAST();
    }

    public void Define(string name, object? value)
    {
        if (variables.ContainsKey(name))
        {
            throw new Exception($"Error: La variable '{name}' ya está definida en el scope actual.");
        }
        variables[name] = value;
    }

    public void Assign(string name, object? value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
        }
        else if (Parent != null)
        {
            Parent.Assign(name, value);
        }
        else
        {
            throw new Exception($"Error: La variable '{name}' no está definida.");
        }
    }

    public object? Get(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else if (Parent != null)
        {
            return Parent.Get(name);
        }
        throw new Exception($"Error: La variable '{name}' no está definida.");
    }

    public bool IsDefined(string name)
    {
        return variables.ContainsKey(name) || (Parent != null && Parent.IsDefined(name));
    }

    public Scope CreateChildScope()
    {
        return new Scope(this, optimizedAST);
    }

    public object? GetOptimizedFunction(string name)
    {
        var optimizedFunction = optimizedAST.GetOptimizedFunction(name);
        if (optimizedFunction != null)
        {
            return optimizedFunction;
        }
        else if (Parent != null)
        {
            return Parent.GetOptimizedFunction(name);
        }
        throw new Exception($"Error: La función '{name}' no está definida o no está optimizada.");
    }
}
