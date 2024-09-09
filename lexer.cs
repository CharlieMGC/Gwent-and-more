using System;
using System.Collections.Generic;
using System.Text;

public class LexerException : Exception
{
    public LexerException(string message) : base(message) { }
}

public static class Lexer
{
    private enum State { Start, Number, Identifier, String, Character, SingleLineComment, MultiLineComment, Done }

    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "if", TokenType.IF }, { "else", TokenType.ELSE }, { "while", TokenType.WHILE }, { "for", TokenType.FOR },
        { "return", TokenType.RETURN }, { "int", TokenType.TYPE }, { "float", TokenType.TYPE }, { "char", TokenType.TYPE },
        { "bool", TokenType.TYPE }, { "void", TokenType.TYPE }, { "string", TokenType.TYPE }, { "true", TokenType.TRUE },
        { "false", TokenType.FALSE }, { "null", TokenType.NULL }, { "break", TokenType.BREAK }, { "continue", TokenType.CONTINUE },
        { "switch", TokenType.SWITCH }, { "case", TokenType.CASE }, { "default", TokenType.DEFAULT }, { "fun", TokenType.FUN },
        { "struct", TokenType.STRUCT }, { "enum", TokenType.ENUM }, { "import", TokenType.IMPORT }, { "try", TokenType.TRY },
        { "catch", TokenType.CATCH }, { "finally", TokenType.FINALLY }, { "throw", TokenType.THROW }
    };

    private static readonly Dictionary<string, TokenType> multiCharTokens = new Dictionary<string, TokenType>
    {
        { "==", TokenType.EQUAL }, { "!=", TokenType.NOT_EQUAL }, { "<=", TokenType.LESS_EQUAL }, { ">=", TokenType.GREATER_EQUAL },
        { "&&", TokenType.AND }, { "||", TokenType.OR }, { "+=", TokenType.PLUS_ASSIGN }, { "-=", TokenType.MINUS_ASSIGN },
        { "*=", TokenType.TIMES_ASSIGN }, { "/=", TokenType.DIVIDE_ASSIGN }, { "%=", TokenType.MODULO_ASSIGN },
        { "&=", TokenType.AND_ASSIGN }, { "|=", TokenType.OR_ASSIGN }, { "^=", TokenType.XOR_ASSIGN }, { "<<=", TokenType.SHIFT_LEFT_ASSIGN },
        { ">>=", TokenType.SHIFT_RIGHT_ASSIGN }, { "++", TokenType.INCREMENT }, { "--", TokenType.DECREMENT },
        { "<<", TokenType.SHIFT_LEFT }, { ">>", TokenType.SHIFT_RIGHT }, { "->", TokenType.ARROW }
    };

    public static List<Token> Tokenize(string text)
    {
        List<Token> tokens = new List<Token>();
        int i = 0, line = 1, column = 1;
        State state = State.Start;
        StringBuilder currentToken = new StringBuilder();
        char[] buffer = text.ToCharArray();

        while (i < buffer.Length)
        {
            char c = buffer[i];
            switch (state)
            {
                case State.Start:
                    if (char.IsWhiteSpace(c)) HandleWhitespace(ref i, ref line, ref column, c);
                    else if (char.IsLetter(c) || c == '_') StartIdentifier(ref state, ref currentToken, c, ref i); // Ajuste en la duplicación de caracteres
                    else if (char.IsDigit(c)) StartNumber(ref state, ref currentToken, c);
                    else if (c == '"') StartString(ref state, ref currentToken);
                    else if (c == '\'') StartCharacter(ref state, ref currentToken);
                    else if (c == '/' && i + 1 < buffer.Length && buffer[i + 1] == '/') // Comentario de una línea
                    {
                        state = State.SingleLineComment;
                        i += 2;
                    }
                    else if (c == '/' && i + 1 < buffer.Length && buffer[i + 1] == '*') // Comentario de varias líneas
                    {
                        state = State.MultiLineComment;
                        i += 2;
                    }
                    else if (i + 1 < buffer.Length && multiCharTokens.ContainsKey($"{c}{buffer[i + 1]}"))
                    {
                        tokens.Add(new Token(multiCharTokens[$"{c}{buffer[i + 1]}"], $"{c}{buffer[i + 1]}", null, line, column));
                        column += 2;
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(HandleSingleCharToken(c, ref column, ref i, tokens));
                    }
                    break;

                case State.Number:
                    HandleNumber(ref state, ref currentToken, ref tokens, ref column, ref i, buffer, c);
                    break;

                case State.Identifier:
                    HandleIdentifier(ref state, ref currentToken, ref tokens, ref column, ref i, buffer, c);
                    break;

                case State.String:
                    HandleString(ref state, ref currentToken, ref tokens, ref column, ref i, buffer, c);
                    break;

                case State.Character:
                    HandleCharacter(ref state, ref currentToken, ref tokens, ref column, ref i, buffer, c);
                    break;

                case State.SingleLineComment: // Ignorar comentarios de una sola línea
                    if (c == '\n')
                    {
                        state = State.Start;
                        line++;
                        column = 1;
                    }
                    i++;
                    break;

                case State.MultiLineComment: // Ignorar comentarios de varias líneas
                    if (c == '*' && i + 1 < buffer.Length && buffer[i + 1] == '/')
                    {
                        state = State.Start;
                        i += 2;
                        column += 2;
                    }
                    else
                    {
                        if (c == '\n')
                        {
                            line++;
                            column = 1;
                        }
                        i++;
                    }
                    break;
            }
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line, column));
        return tokens;
    }

    private static void HandleWhitespace(ref int i, ref int line, ref int column, char c)
    {
        if (c == '\n')
        {
            line++;
            column = 1;
        }
        else
        {
            column++;
        }
        i++;
    }

    private static void StartIdentifier(ref State state, ref StringBuilder currentToken, char c, ref int i)
    {
        state = State.Identifier;
        currentToken.Append(c);
        i++; // Avanzar el índice aquí para evitar duplicar la primera letra
    }

    private static void StartNumber(ref State state, ref StringBuilder currentToken, char c)
    {
        state = State.Number;
        currentToken.Append(c);
    }

    private static void StartString(ref State state, ref StringBuilder currentToken)
    {
        state = State.String;
    }

    private static void StartCharacter(ref State state, ref StringBuilder currentToken)
    {
        state = State.Character;
    }

    private static Token HandleSingleCharToken(char c, ref int column, ref int i, List<Token> tokens)
    {
        TokenType type = c switch
        {
            '+' => TokenType.PLUS,
            '-' => TokenType.MINUS,
            '*' => TokenType.TIMES,
            '/' => TokenType.DIVIDE,
            '%' => TokenType.MODULO,
            '=' => TokenType.ASSIGN,
            '<' => TokenType.LESS_THAN,
            '>' => TokenType.GREATER_THAN,
            '&' => TokenType.BIT_AND,
            '|' => TokenType.BIT_OR,
            '^' => TokenType.BIT_XOR,
            '~' => TokenType.BIT_NOT,
            '.' => TokenType.DOT,
            ',' => TokenType.COMMA,
            ';' => TokenType.SEMICOLON,
            ':' => TokenType.COLON,
            '(' => TokenType.LPAREN,
            ')' => TokenType.RPAREN,
            '{' => TokenType.LBRACE,
            '}' => TokenType.RBRACE,
            '[' => TokenType.LBRACKET,
            ']' => TokenType.RBRACKET,
            '?' => TokenType.QUESTION,
            '#' => TokenType.HASH,
            '$' => TokenType.DOLLAR,
            '\\' => TokenType.BACKSLASH,
            '`' => TokenType.BACKTICK,
            _ => throw new LexerException($"Unexpected character: {c}")
        };
        column++;
        i++;
        return new Token(type, c.ToString(), null, 1, column);
    }

    private static void HandleNumber(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int column, ref int i, char[] buffer, char c)
    {
        if (char.IsDigit(c))
        {
            currentToken.Append(c);
        }
        else
        {
            tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString(), int.Parse(currentToken.ToString()), 1, column));
            state = State.Start;
            currentToken.Clear();
        }
        i++;
        column++;
    }

    private static void HandleIdentifier(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int column, ref int i, char[] buffer, char c)
    {
        if (char.IsLetterOrDigit(c))
        {
            currentToken.Append(c);
        }
        else
        {
            string identifier = currentToken.ToString();
            tokens.Add(new Token(keywords.TryGetValue(identifier, out var keywordType) ? keywordType : TokenType.ID, identifier, null, 1, column));
            state = State.Start;
            currentToken.Clear();
        }
        i++;
        column++;
    }

    private static void HandleString(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int column, ref int i, char[] buffer, char c)
    {
        if (c == '"' && buffer[i - 1] != '\\')
        {
            tokens.Add(new Token(TokenType.STRING, currentToken.ToString(), currentToken.ToString(), 1, column));
            state = State.Start;
            currentToken.Clear();
        }
        else
        {
            currentToken.Append(c);
        }
        i++;
        column++;
    }

    private static void HandleCharacter(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int column, ref int i, char[] buffer, char c)
    {
        if (c == '\'' && buffer[i - 1] != '\\')
        {
            tokens.Add(new Token(TokenType.CHARACTER, currentToken.ToString(), currentToken.ToString(), 1, column));
            state = State.Start;
            currentToken.Clear();
        }
        else
        {
            currentToken.Append(c);
        }
        i++;
        column++;
    }
}
