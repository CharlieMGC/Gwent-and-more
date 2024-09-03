using System;
using System.Collections.Generic;

public class Scope
{
    private readonly Dictionary<string, object> variables;
    private readonly Scope? parentScope;

    public Scope(Scope? parentScope = null)
    {
        this.parentScope = parentScope;
        this.variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    public void Define(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Variable '{name}' is already defined in this scope.");
        }
        variables[name] = value;
    }

    public object Get(string name)
    {
        if (variables.TryGetValue(name, out object? value))
        {
            return value;
        }
        if (parentScope != null)
        {
            return parentScope.Get(name);
        }
        throw new KeyNotFoundException($"Variable '{name}' is not defined.");
    }

    public void Assign(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return;
        }
        if (parentScope != null)
        {
            parentScope.Assign(name, value);
            return;
        }
        throw new KeyNotFoundException($"Variable '{name}' is not defined.");
    }

    public bool IsDefined(string name)
    {
        return variables.ContainsKey(name) || (parentScope?.IsDefined(name) ?? false);
    }

    public bool TryGet(string name, out object? value)
    {
        if (variables.TryGetValue(name, out value))
        {
            return true;
        }
        if (parentScope != null)
        {
            return parentScope.TryGet(name, out value);
        }
        value = null;
        return false;
    }
}