public enum TokenType
{
    // Literales
    NUMBER,
    FLOAT,
    HEX_NUMBER,
    BIN_NUMBER,
    STRING,
    CHARACTER,
    BOOLEAN,
    NULL,

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
    LESS_EQUAL,  // <=
    GREATER_EQUAL, // >=
    AND,        // &&
    OR,         // ||
    NOT,        // !
    ASSIGN,     // =
    PLUS_ASSIGN, // +=
    MINUS_ASSIGN, // -=
    TIMES_ASSIGN, // *=
    DIVIDE_ASSIGN, // /=

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

    // Otros Operadores de Asignación
    MODULO_ASSIGN, // %=
    AND_ASSIGN,    // &=
    OR_ASSIGN,     // |=
    XOR_ASSIGN,    // ^=
    SHIFT_LEFT_ASSIGN, // <<=
    SHIFT_RIGHT_ASSIGN, // >>=

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

    // Comentarios
    COMMENT, // // o /* ... */

    // Palabras clave
    KEYWORD, // if, else, while, for, return, etc.

    // Nueva línea
    NEWLINE,

    // Espacios en blanco
    WHITESPACE,
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
