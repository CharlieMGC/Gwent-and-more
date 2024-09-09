using System;

public enum TokenType
{
    // Literales
    NUMBER, FLOAT, HEX_NUMBER, BIN_NUMBER, STRING, CHARACTER, TRUE, FALSE, NULL,

    // Identificadores
    ID,

    // Operadores aritméticos
    PLUS, MINUS, TIMES, DIVIDE, MODULO,

    // Operadores de comparación
    EQUAL, NOT_EQUAL, LESS_THAN, GREATER_THAN, LESS_EQUAL, GREATER_EQUAL,

    // Operadores lógicos
    AND, OR, NOT,

    // Operadores de asignación
    ASSIGN, PLUS_ASSIGN, MINUS_ASSIGN, TIMES_ASSIGN, DIVIDE_ASSIGN, MODULO_ASSIGN,
    AND_ASSIGN, OR_ASSIGN, XOR_ASSIGN, SHIFT_LEFT_ASSIGN, SHIFT_RIGHT_ASSIGN,

    // Operadores de incremento y decremento
    INCREMENT, DECREMENT,

    // Operadores bit a bit
    BIT_AND, BIT_OR, BIT_XOR, BIT_NOT, SHIFT_LEFT, SHIFT_RIGHT, ARROW,

    // Separadores
    COMMA, SEMICOLON, COLON, DOT, QUESTION, HASH, DOLLAR, BACKSLASH, BACKTICK,

    // Corchetes
    LPAREN, RPAREN, LBRACE, RBRACE, LBRACKET, RBRACKET,

    // Palabras clave
    IF, ELSE, WHILE, FOR, RETURN, FUN, TYPE, STRUCT, ENUM, IMPORT, TRY, CATCH, FINALLY, THROW,
    BREAK, CONTINUE, SWITCH, CASE, DEFAULT,

    // Otros
    EOF
}

public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }
    public object? Literal { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenType type, string lexeme, object? literal, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
        Column = column;
    }

    public override string ToString() => $"{Type} {Lexeme} {Literal} (Line: {Line}, Column: {Column})";

    public override bool Equals(object? obj) =>
        obj is Token other && Type == other.Type && Lexeme == other.Lexeme &&
        Equals(Literal, other.Literal) && Line == other.Line && Column == other.Column;

    public override int GetHashCode() => HashCode.Combine(Type, Lexeme, Literal, Line, Column);

    // Improvement: Adding method to get more detailed error information
    public static string GetDetailedErrorInfo(Token token, string message)
    {
        return $"Error at line {token.Line}, column {token.Column}: {message}. Token: {token.Lexeme}";
    }
}
