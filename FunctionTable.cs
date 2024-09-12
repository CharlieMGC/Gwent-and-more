using System;
using System.Collections.Generic;

// Esta clase maneja todas las funciones definidas en el programa
public class FunctionTable
{
    // Diccionario para almacenar las funciones, usando su nombre como clave
    private Dictionary<string, FunctionDeclaration> functions;

    // Constructor que inicializa el diccionario
    public FunctionTable()
    {
        functions = new Dictionary<string, FunctionDeclaration>();
    }

    // Método para definir (registrar) una función en la tabla
    public void DefineFunction(string name, FunctionDeclaration declaration)
    {
        // Verificar si la función ya está definida para evitar duplicados
        if (functions.ContainsKey(name))
        {
            throw new Exception($"Error: La función '{name}' ya está definida.");
        }

        // Registrar la función
        functions[name] = declaration;
    }

    // Método para obtener una función por su nombre
    public FunctionDeclaration GetFunction(string name)
    {
        // Verificar si la función existe en la tabla
        if (!functions.TryGetValue(name, out var function))
        {
            throw new Exception($"Error: La función '{name}' no está definida.");
        }

        return function;
    }

    // Método para verificar si una función está definida
    public bool IsDefined(string name)
    {
        return functions.ContainsKey(name);
    }

    // Método para imprimir todas las funciones almacenadas (opcional, útil para debugging)
    public void PrintFunctions()
    {
        Console.WriteLine("Funciones definidas:");
        foreach (var func in functions)
        {
            Console.WriteLine($" - {func.Key}");
        }
    }
}
