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

    // Define a new variable in the current scope
    public void Define(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Variable '{name}' is already defined in this scope.");
        }
        variables[name] = value;
    }

    // Get the value of a variable, searching in the current and parent scopes
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

    // Assign a value to a variable, searching in the current and parent scopes
    public void Assign(string name, object value)
    {
        if (TryAssign(name, value))
        {
            return;
        }
        throw new KeyNotFoundException($"Variable '{name}' is not defined.");
    }

    // Check if a variable is defined in the current or parent scopes
    public bool IsDefined(string name)
    {
        return variables.ContainsKey(name) || (parentScope?.IsDefined(name) ?? false);
    }

    // Try to get the value of a variable, returning false if not found
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

    // Try to assign a value to a variable, returning false if the variable is not found
    public bool TryAssign(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return true;
        }
        if (parentScope != null)
        {
            return parentScope.TryAssign(name, value);
        }
        return false;
    }

    // Create a new child scope
    public Scope CreateChildScope()
    {
        return new Scope(this);
    }
}
