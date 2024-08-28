using System.Collections.Generic;

public class Scope
{
    private readonly Dictionary<string, object> variables = new Dictionary<string, object>();
    private readonly Scope? parentScope;

    public Scope(Scope? parentScope = null)
    {
        this.parentScope = parentScope;
    }

    public void Define(string name, object value)
    {
        variables[name] = value;
    }

    public object Get(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else if (parentScope != null)
        {
            return parentScope.Get(name);
        }
        else
        {
            throw new Exception($"Variable '{name}' no definida.");
        }
    }

    public void Assign(string name, object value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
        }
        else if (parentScope != null)
        {
            parentScope.Assign(name, value);
        }
        else
        {
            throw new Exception($"Variable '{name}' no definida.");
        }
    }
}
