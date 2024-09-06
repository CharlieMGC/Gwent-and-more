using System;
using System.Collections.Generic;

public class Scope
{
    private readonly Dictionary<string, ScopeVariable> variables;
    private readonly Scope? parentScope;

    public Scope(Scope? parentScope = null)
    {
        this.parentScope = parentScope;
        this.variables = new Dictionary<string, ScopeVariable>(StringComparer.OrdinalIgnoreCase);
    }

    // Estructura para almacenar información de la variable
    private class ScopeVariable
    {
        public object? Value { get; set; }
        public bool IsParameter { get; set; }
    }

    // Define una nueva variable en el ámbito actual
    public void Define(string name, object? value, bool isParameter = false)
    {
        variables[name] = new ScopeVariable { Value = value, IsParameter = isParameter };
    }

    // Obtiene el valor de una variable, buscando en los ámbitos actuales y superiores
    public object? Get(string name)
    {
        if (variables.TryGetValue(name, out var variable))
        {
            return variable.Value;
        }
        if (parentScope != null)
        {
            return parentScope.Get(name);
        }
        throw new KeyNotFoundException($"Variable '{name}' is not defined.");
    }

    // Asigna un valor a una variable, buscando en los ámbitos actuales y superiores
    public void Assign(string name, object value)
    {
        if (variables.TryGetValue(name, out var variable))
        {
            variable.Value = value;
        }
        else if (parentScope != null)
        {
            parentScope.Assign(name, value);
        }
        else
        {
            throw new KeyNotFoundException($"Variable '{name}' is not defined.");
        }
    }

    // Verifica si una variable está definida en los ámbitos actuales o superiores
    public bool IsDefined(string name)
    {
        return variables.ContainsKey(name) || (parentScope?.IsDefined(name) ?? false);
    }

    // Intenta obtener el valor de una variable, devolviendo false si no se encuentra
    public bool TryGet(string name, out object? value)
    {
        if (variables.TryGetValue(name, out var variable))
        {
            value = variable.Value;
            return true;
        }
        if (parentScope != null)
        {
            return parentScope.TryGet(name, out value);
        }
        value = null;
        return false;
    }

    // Crea un nuevo ámbito hijo
    public Scope CreateChildScope()
    {
        return new Scope(this);
    }

    // Método para definir parámetros de función
    public void DefineParameter(string name, object? value)
    {
        Define(name, value, true);
    }

    // Método para verificar si una variable es un parámetro
    public bool IsParameter(string name)
    {
        return variables.TryGetValue(name, out var variable) && variable.IsParameter;
    }
}