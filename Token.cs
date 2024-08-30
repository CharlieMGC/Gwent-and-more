using System;

public enum TokenType
{
    // Literales
    NUMBER,
    FLOAT,
    HEX_NUMBER,
    BIN_NUMBER,
    STRING,
    CHARACTER,
    TRUE,
    FALSE,
    NULL,

    // Identificadores
    ID,

    // Operadores aritméticos
    PLUS,       // +
    MINUS,      // -
    TIMES,      // *
    DIVIDE,     // /
    MODULO,     // %

    // Operadores de comparación
    EQUAL,      // ==
    NOT_EQUAL,  // !=
    LESS_THAN,  // <
    GREATER_THAN, // >
    LESS_EQUAL,  // <=
    GREATER_EQUAL, // >=

    // Operadores lógicos
    AND,        // &&
    OR,         // ||
    NOT,        // !

    // Operadores de asignación
    ASSIGN,     // =
    PLUS_ASSIGN, // +=
    MINUS_ASSIGN, // -=
    TIMES_ASSIGN, // *=
    DIVIDE_ASSIGN, // /=
    MODULO_ASSIGN, // %=
    AND_ASSIGN,    // &=
    OR_ASSIGN,     // |=
    XOR_ASSIGN,    // ^=
    SHIFT_LEFT_ASSIGN, // <<=
    SHIFT_RIGHT_ASSIGN, // >>=

    // Operadores de Incremento y Decremento
    INCREMENT,  // ++
    DECREMENT,  // --

    // Operadores Bit a Bit
    BIT_AND,    // &
    BIT_OR,     // |
    BIT_XOR,    // ^
    BIT_NOT,    // ~
    SHIFT_LEFT, // <<
    SHIFT_RIGHT, // >>

    // Separadores
    COMMA,      // ,
    SEMICOLON,  // ;
    COLON,      // :
    DOT,        // .
    QUESTION,   // ?
    HASH,       // #
    DOLLAR,     // $
    BACKSLASH,  // \
    BACKTICK,   // `

    // Corchetes
    LPAREN,     // (
    RPAREN,     // )
    LBRACE,     // {
    RBRACE,     // }
    LBRACKET,   // [
    RBRACKET,   // ]

    // Palabras clave
    IF,
    ELSE,
    WHILE,
    FOR,
    RETURN,
    FUN,        // Function declaration
    TYPE,       // int, float, char, bool, void, string

    // Control de flujo
    BREAK,
    CONTINUE,
    SWITCH,
    CASE,
    DEFAULT,

    // Otros
    NEWLINE,
    EOF,
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

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal} (Line: {Line}, Column: {Column})";
    }
}