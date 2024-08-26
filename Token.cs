public enum TokenType
{
    // Literales
    NUMBER,
    STRING,
    CHARACTER,

    // Identificadores
    ID,

    // Operadores
    PLUS,       // +
    MINUS,      // -
    TIMES,      // *
    DIVIDE,     // /
    MODULO,     // %
    EQUAL,      // ==
    NOT_EQUAL,  // !=
    LESS_THAN,  // <
    GREATER_THAN, // >
    LESS_EQUAL, // <=
    GREATER_EQUAL, // >=
    AND,        // &&
    OR,         // ||
    NOT,        // !
    ASSIGN,     // =
    PLUS_ASSIGN, // +=
    MINUS_ASSIGN, // -=
    TIMES_ASSIGN, // *=
    DIVIDE_ASSIGN, // /=

    // Separadores
    COMMA,      // ,
    SEMICOLON,  // ;
    COLON,      // :

    // Corchetes
    LPAREN,     // (
    RPAREN,     // )
    LBRACE,     // {
    RBRACE,     // }
    LBRACKET,   // [
    RBRACKET,   // ]

    // Comentarios
    COMMENT,    // // o /* ... */

    // Espacios en blanco
    WHITESPACE,

    // Palabras clave
    KEYWORD,    // if, else, while, for, return, etc.

    // Desconocido
    UNKNOWN
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}
