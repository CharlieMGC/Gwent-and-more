public enum TokenType
{
    NUMBER,
    ID,
    PLUS,
    MINUS,
    TIMES,
    DIVIDE,
    LPAREN,
    RPAREN,
    WHITESPACE,
    LBRACE,
    RBRACE,
    COLON,
    QUOTE,
    UNKNOWN // Nuevo tipo de token
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
