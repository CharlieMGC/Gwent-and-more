using System;
using System.Collections.Generic;
using System.Text;

public class LexerException : Exception
{
    public LexerException(string message) : base(message) { }
}

public static class Lexer
{
    private enum State
    {
        Start,
        Number,
        Identifier,
        String,
        Character,
        SingleLineComment,
        MultiLineComment,
        Done
    }

    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "if", TokenType.IF },
        { "else", TokenType.ELSE },
        { "while", TokenType.WHILE },
        { "for", TokenType.FOR },
        { "return", TokenType.RETURN },
        { "int", TokenType.TYPE },
        { "float", TokenType.TYPE },
        { "char", TokenType.TYPE },
        { "bool", TokenType.TYPE },
        { "void", TokenType.TYPE },
        { "string", TokenType.TYPE },
        { "true", TokenType.TRUE },
        { "false", TokenType.FALSE },
        { "null", TokenType.NULL },
        { "break", TokenType.BREAK },
        { "continue", TokenType.CONTINUE },
        { "switch", TokenType.SWITCH },
        { "case", TokenType.CASE },
        { "default", TokenType.DEFAULT },
        { "fun", TokenType.FUN },
        { "struct", TokenType.STRUCT },
        { "enum", TokenType.ENUM },
        { "import", TokenType.IMPORT },
        { "try", TokenType.TRY },
        { "catch", TokenType.CATCH },
        { "finally", TokenType.FINALLY },
        { "throw", TokenType.THROW }
    };

    private static readonly Dictionary<string, TokenType> multiCharTokens = new Dictionary<string, TokenType>
    {
        { "==", TokenType.EQUAL },
        { "!=", TokenType.NOT_EQUAL },
        { "<=", TokenType.LESS_EQUAL },
        { ">=", TokenType.GREATER_EQUAL },
        { "&&", TokenType.AND },
        { "||", TokenType.OR },
        { "+=", TokenType.PLUS_ASSIGN },
        { "-=", TokenType.MINUS_ASSIGN },
        { "*=", TokenType.TIMES_ASSIGN },
        { "/=", TokenType.DIVIDE_ASSIGN },
        { "%=", TokenType.MODULO_ASSIGN },
        { "&=", TokenType.AND_ASSIGN },
        { "|=", TokenType.OR_ASSIGN },
        { "^=", TokenType.XOR_ASSIGN },
        { "<<=", TokenType.SHIFT_LEFT_ASSIGN },
        { ">>=", TokenType.SHIFT_RIGHT_ASSIGN },
        { "++", TokenType.INCREMENT },
        { "--", TokenType.DECREMENT },
        { "<<", TokenType.SHIFT_LEFT },
        { ">>", TokenType.SHIFT_RIGHT },
        { "->", TokenType.ARROW }
    };

    public static List<Token> Tokenize(string text)
    {
        text = Preprocess(text);
        List<Token> tokens = new List<Token>();
        int i = 0;
        State state = State.Start;
        StringBuilder currentToken = new StringBuilder();
        char[] buffer = text.ToCharArray();
        int line = 1;
        int column = 1;

        while (i < buffer.Length)
        {
            char c = buffer[i];

            switch (state)
            {
                case State.Start:
                    if (char.IsWhiteSpace(c))
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
                        continue;
                    }
                    if (char.IsLetter(c) || c == '_')
                    {
                        state = State.Identifier;
                        currentToken.Append(c);
                    }
                    else if (char.IsDigit(c))
                    {
                        state = State.Number;
                        currentToken.Append(c);
                    }
                    else if (c == '"')
                    {
                        state = State.String;
                        currentToken.Append(c);
                    }
                    else if (c == '\'')
                    {
                        state = State.Character;
                        currentToken.Append(c);
                    }
                    else if (c == '/' && i + 1 < buffer.Length)
                    {
                        if (buffer[i + 1] == '/')
                        {
                            state = State.SingleLineComment;
                            i++; // Skip next '/'
                        }
                        else if (buffer[i + 1] == '*')
                        {
                            state = State.MultiLineComment;
                            i++; // Skip next '*'
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.DIVIDE, c.ToString(), null, line, column));
                        }
                    }
                    else if (i + 1 < buffer.Length && multiCharTokens.ContainsKey($"{c}{buffer[i + 1]}"))
                    {
                        tokens.Add(new Token(multiCharTokens[$"{c}{buffer[i + 1]}"], $"{c}{buffer[i + 1]}", null, line, column));
                        column += 2;
                        i += 2;
                        continue;
                    }
                    else
                    {
                        TokenType type = GetSingleCharTokenType(c);
                        tokens.Add(new Token(type, c.ToString(), null, line, column));
                    }
                    column++;
                    i++;
                    break;

                case State.Number:
                    if (char.IsDigit(c) || c == '.' || c == 'x' || c == 'b')
                    {
                        currentToken.Append(c);
                        i++;
                        column++;
                    }
                    else
                    {
                        string number = currentToken.ToString();
                        TokenType numberType = DetermineNumberType(number);
                        tokens.Add(new Token(numberType, number, ParseNumber(number, numberType), line, column - number.Length));
                        currentToken.Clear();
                        state = State.Start;
                    }
                    break;

                case State.Identifier:
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
                        currentToken.Clear();
                        state = State.Start;
                    }
                    break;

                case State.String:
                    if (c == '"' && buffer[i - 1] != '\\')
                    {
                        currentToken.Append(c);
                        tokens.Add(new Token(TokenType.STRING, currentToken.ToString(), currentToken.ToString().Substring(1, currentToken.Length - 2), line, column - currentToken.Length));
                        currentToken.Clear();
                        state = State.Start;
                        i++;
                        column++;
                    }
                    else
                    {
                        currentToken.Append(c);
                        i++;
                        column++;
                    }
                    break;

                case State.Character:
                    if (c == '\'' && buffer[i - 1] != '\\')
                    {
                        currentToken.Append(c);
                        tokens.Add(new Token(TokenType.CHARACTER, currentToken.ToString(), currentToken.ToString()[1], line, column - currentToken.Length));
                        currentToken.Clear();
                        state = State.Start;
                        i++;
                        column++;
                    }
                    else
                    {
                        currentToken.Append(c);
                        i++;
                        column++;
                    }
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

        if (currentToken.Length > 0)
        {
            throw new LexerException($"Unexpected end of input: {currentToken}");
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line, column));
        return tokens;
    }

    private static TokenType GetSingleCharTokenType(char c)
    {
        switch (c)
        {
            case '+': return TokenType.PLUS;
            case '-': return TokenType.MINUS;
            case '*': return TokenType.TIMES;
            case '/': return TokenType.DIVIDE;
            case '%': return TokenType.MODULO;
            case '=': return TokenType.ASSIGN;
            case '<': return TokenType.LESS_THAN;
            case '>': return TokenType.GREATER_THAN;
            case '!': return TokenType.NOT;
            case '&': return TokenType.BIT_AND;
            case '|': return TokenType.BIT_OR;
            case '^': return TokenType.BIT_XOR;
            case '~': return TokenType.BIT_NOT;
            case '.': return TokenType.DOT;
            case ',': return TokenType.COMMA;
            case ';': return TokenType.SEMICOLON;
            case ':': return TokenType.COLON;
            case '(': return TokenType.LPAREN;
            case ')': return TokenType.RPAREN;
            case '{': return TokenType.LBRACE;
            case '}': return TokenType.RBRACE;
            case '[': return TokenType.LBRACKET;
            case ']': return TokenType.RBRACKET;
            case '?': return TokenType.QUESTION;
            case '#': return TokenType.HASH;
            case '$': return TokenType.DOLLAR;
            case '\\': return TokenType.BACKSLASH;
            case '`': return TokenType.BACKTICK;
            default: throw new LexerException($"Unexpected character: {c}");
        }
    }

    private static TokenType DetermineNumberType(string number)
    {
        if (number.Contains("."))
            return TokenType.FLOAT;
        if (number.StartsWith("0x") || number.StartsWith("0X"))
            return TokenType.HEX_NUMBER;
        if (number.StartsWith("0b") || number.StartsWith("0B"))
            return TokenType.BIN_NUMBER;
        return TokenType.NUMBER;
    }

    private static object ParseNumber(string number, TokenType type)
    {
        switch (type)
        {
            case TokenType.FLOAT:
                return double.Parse(number);
            case TokenType.HEX_NUMBER:
                return Convert.ToInt32(number.Substring(2), 16);
            case TokenType.BIN_NUMBER:
                return Convert.ToInt32(number.Substring(2), 2);
            case TokenType.NUMBER:
                return int.Parse(number);
            default:
                throw new LexerException($"Invalid number type: {type}");
        }
    }

    private static string Preprocess(string text)
    {
        StringBuilder result = new StringBuilder();
        bool inMultiLineComment = false;
        int i = 0;

        while (i < text.Length)
        {
            if (!inMultiLineComment && text[i] == '/' && i + 1 < text.Length && text[i + 1] == '*')
            {
                inMultiLineComment = true;
                i += 2;
            }
            else if (inMultiLineComment && text[i] == '*' && i + 1 < text.Length && text[i + 1] == '/')
            {
                inMultiLineComment = false;
                i += 2;
            }
            else if (!inMultiLineComment)
            {
                result.Append(text[i]);
                i++;
            }
            else
            {
                i++;
            }
        }

        return result.ToString();
    }
}