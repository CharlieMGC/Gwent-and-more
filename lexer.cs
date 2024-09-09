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
        { "catch", TokenType.CATCH }, { "finally", TokenType.FINALLY }, { "throw", TokenType.THROW },
        { "class", TokenType.CLASS }, { "extends", TokenType.EXTENDS }, { "public", TokenType.PUBLIC },
        { "private", TokenType.PRIVATE }, { "protected", TokenType.PROTECTED }
    };

    private static readonly Dictionary<string, TokenType> multiCharTokens = new Dictionary<string, TokenType>
    {
        { "==", TokenType.EQUAL }, { "!=", TokenType.NOT_EQUAL }, { "<=", TokenType.LESS_EQUAL }, { ">=", TokenType.GREATER_EQUAL },
        { "&&", TokenType.AND }, { "||", TokenType.OR }, { "+=", TokenType.PLUS_ASSIGN }, { "-=", TokenType.MINUS_ASSIGN },
        { "*=", TokenType.TIMES_ASSIGN }, { "/=", TokenType.DIVIDE_ASSIGN }, { "%=", TokenType.MODULO_ASSIGN },
        { "&=", TokenType.AND_ASSIGN }, { "|=", TokenType.OR_ASSIGN }, { "^=", TokenType.XOR_ASSIGN },
        { "<<=", TokenType.SHIFT_LEFT_ASSIGN }, { ">>=", TokenType.SHIFT_RIGHT_ASSIGN },
        { "++", TokenType.INCREMENT }, { "--", TokenType.DECREMENT }, { "<<", TokenType.SHIFT_LEFT },
        { ">>", TokenType.SHIFT_RIGHT }, { "->", TokenType.ARROW }
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
                    if (char.IsWhiteSpace(c))
                    {
                        HandleWhitespace(ref i, ref line, ref column, c);
                    }
                    else if (char.IsLetter(c) || c == '_')
                    {
                        StartIdentifier(ref state, ref currentToken, c, ref i, ref column);
                    }
                    else if (char.IsDigit(c))
                    {
                        StartNumber(ref state, ref currentToken, c, ref i, ref column);
                    }
                    else if (c == '"')
                    {
                        StartString(ref state, ref i, ref column);
                    }
                    else if (c == '\'')
                    {
                        StartCharacter(ref state, ref i, ref column);
                    }
                    else if (c == '/' && i + 1 < buffer.Length && buffer[i + 1] == '/')
                    {
                        state = State.SingleLineComment;
                        i += 2;
                        column += 2;
                    }
                    else if (c == '/' && i + 1 < buffer.Length && buffer[i + 1] == '*')
                    {
                        state = State.MultiLineComment;
                        i += 2;
                        column += 2;
                    }
                    else if (i + 1 < buffer.Length && multiCharTokens.ContainsKey($"{c}{buffer[i + 1]}"))
                    {
                        tokens.Add(new Token(multiCharTokens[$"{c}{buffer[i + 1]}"], $"{c}{buffer[i + 1]}", null, line, column));
                        column += 2;
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(HandleSingleCharToken(c, line, column));
                        i++;
                        column++;
                    }
                    break;

                case State.Number:
                    HandleNumber(ref state, ref currentToken, ref tokens, ref line, ref column, ref i, buffer, c);
                    break;

                case State.Identifier:
                    HandleIdentifier(ref state, ref currentToken, ref tokens, ref line, ref column, ref i, buffer, c);
                    break;

                case State.String:
                    HandleString(ref state, ref currentToken, ref tokens, ref line, ref column, ref i, buffer, c);
                    break;

                case State.Character:
                    HandleCharacter(ref state, ref currentToken, ref tokens, ref line, ref column, ref i, buffer, c);
                    break;

                case State.SingleLineComment:
                    if (c == '\n')
                    {
                        state = State.Start;
                        line++;
                        column = 1;
                    }
                    i++;
                    break;

                case State.MultiLineComment:
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
                        else
                        {
                            column++;
                        }
                        i++;
                    }
                    break;
            }
        }

        if (state == State.Number || state == State.Identifier)
        {
            AddFinalToken(ref tokens, state, currentToken, line, column);
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

    private static void StartIdentifier(ref State state, ref StringBuilder currentToken, char c, ref int i, ref int column)
    {
        state = State.Identifier;
        currentToken.Append(c);
        i++;
        column++;
    }

    private static void StartNumber(ref State state, ref StringBuilder currentToken, char c, ref int i, ref int column)
    {
        state = State.Number;
        currentToken.Append(c);
        i++;
        column++;
    }

    private static void StartString(ref State state, ref int i, ref int column)
    {
        state = State.String;
        i++;
        column++;
    }

    private static void StartCharacter(ref State state, ref int i, ref int column)
    {
        state = State.Character;
        i++;
        column++;
    }

    private static Token HandleSingleCharToken(char c, int line, int column)
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
        return new Token(type, c.ToString(), null, line, column);
    }

    private static void HandleNumber(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int line, ref int column, ref int i, char[] buffer, char c)
    {
        if (char.IsDigit(c))
        {
            currentToken.Append(c);
            i++;
            column++;
        }
        else
        {
            if (currentToken.Length > 0)
            {
                tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString(), int.Parse(currentToken.ToString()), line, column - currentToken.Length));
                state = State.Start;
                currentToken.Clear();
            }
        }
    }
    private static void HandleIdentifier(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int line, ref int column, ref int i, char[] buffer, char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_')
        {
            currentToken.Append(c);
            i++;
            column++;
        }
        else
        {
            string identifier = currentToken.ToString();
            if (keywords.TryGetValue(identifier, out TokenType keywordType))
            {
                tokens.Add(new Token(keywordType, identifier, null, line, column - identifier.Length));
            }
            else
            {
                tokens.Add(new Token(TokenType.ID, identifier, null, line, column - identifier.Length));
            }
            state = State.Start;
            currentToken.Clear();
            // No incrementamos i aquí para que el próximo carácter se procese en el siguiente ciclo
        }
    }
    private static void HandleString(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int line, ref int column, ref int i, char[] buffer, char c)
    {
        if (c == '"' && buffer[i - 1] != '\\')
        {
            tokens.Add(new Token(TokenType.STRING, currentToken.ToString(), currentToken.ToString(), line, column - currentToken.Length - 1));
            state = State.Start;
            currentToken.Clear();
            i++;
            column++;
        }
        else
        {
            currentToken.Append(c);
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
    }

    private static void HandleCharacter(ref State state, ref StringBuilder currentToken, ref List<Token> tokens, ref int line, ref int column, ref int i, char[] buffer, char c)
    {
        if (c == '\'' && buffer[i - 1] != '\\')
        {
            tokens.Add(new Token(TokenType.CHARACTER, currentToken.ToString(), currentToken.ToString(), line, column - currentToken.Length - 1));
            state = State.Start;
            currentToken.Clear();
            i++;
            column++;
        }
        else
        {
            currentToken.Append(c);
            i++;
            column++;
        }
    }

    private static void AddFinalToken(ref List<Token> tokens, State state, StringBuilder currentToken, int line, int column)
    {
        if (state == State.Number)
        {
            tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString(), int.Parse(currentToken.ToString()), line, column - currentToken.Length));
        }
        else if (state == State.Identifier)
        {
            string identifier = currentToken.ToString();
            if (keywords.TryGetValue(identifier, out TokenType keywordType))
            {
                tokens.Add(new Token(keywordType, identifier, null, line, column - identifier.Length));
            }
            else
            {
                tokens.Add(new Token(TokenType.ID, identifier, null, line, column - identifier.Length));
            }
        }
    }
}