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
        Comment,
        Done
    }

    private static readonly Dictionary<char, TokenType> singleCharTokens = new Dictionary<char, TokenType>
    {
        { '+', TokenType.PLUS }, { '-', TokenType.MINUS }, { '*', TokenType.TIMES }, { '/', TokenType.DIVIDE },
        { '%', TokenType.MODULO }, { '=', TokenType.ASSIGN }, { '!', TokenType.NOT }, { '<', TokenType.LESS_THAN },
        { '>', TokenType.GREATER_THAN }, { '&', TokenType.BIT_AND }, { '|', TokenType.BIT_OR }, { '^', TokenType.BIT_XOR },
        { '~', TokenType.BIT_NOT }, { '.', TokenType.DOT }, { '?', TokenType.QUESTION }, { '#', TokenType.HASH },
        { '$', TokenType.DOLLAR }, { '\\', TokenType.BACKSLASH }, { '`', TokenType.BACKTICK }, { ',', TokenType.COMMA },
        { ';', TokenType.SEMICOLON }, { ':', TokenType.COLON }, { '(', TokenType.LPAREN }, { ')', TokenType.RPAREN },
        { '{', TokenType.LBRACE }, { '}', TokenType.RBRACE }, { '[', TokenType.LBRACKET }, { ']', TokenType.RBRACKET },
        { '\n', TokenType.NEWLINE }
    };

    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "if", TokenType.KEYWORD }, { "else", TokenType.KEYWORD }, { "while", TokenType.KEYWORD },
        { "for", TokenType.KEYWORD }, { "return", TokenType.KEYWORD }, { "int", TokenType.KEYWORD },
        { "float", TokenType.KEYWORD }, { "char", TokenType.KEYWORD }, { "bool", TokenType.KEYWORD },
        { "void", TokenType.KEYWORD }, { "string", TokenType.KEYWORD }
    };

    private static readonly Dictionary<string, TokenType> multiCharTokens = new Dictionary<string, TokenType>
    {
        { "==", TokenType.EQUAL }, { "!=", TokenType.NOT_EQUAL }, { "<=", TokenType.LESS_EQUAL },
        { ">=", TokenType.GREATER_EQUAL }, { "&&", TokenType.AND }, { "||", TokenType.OR },
        { "+=", TokenType.PLUS_ASSIGN }, { "-=", TokenType.MINUS_ASSIGN }, { "*=", TokenType.TIMES_ASSIGN },
        { "/=", TokenType.DIVIDE_ASSIGN }, { "++", TokenType.INCREMENT }, { "--", TokenType.DECREMENT },
        { "%=", TokenType.MODULO_ASSIGN }, { "&=", TokenType.AND_ASSIGN }, { "|=", TokenType.OR_ASSIGN },
        { "^=", TokenType.XOR_ASSIGN }, { "<<", TokenType.SHIFT_LEFT }, { ">>", TokenType.SHIFT_RIGHT },
        { "<<=", TokenType.SHIFT_LEFT_ASSIGN }, { ">>=", TokenType.SHIFT_RIGHT_ASSIGN }
    };

    public static List<Token> Tokenizer(string texto)
    {
        texto = Preprocess(texto);
        List<Token> tokens = new List<Token>();
        int i = 0;
        State state = State.Start;
        StringBuilder currentToken = new StringBuilder();
        char[] buffer = texto.ToCharArray();
        int line = 1;

        while (i < buffer.Length)
        {
            char c = buffer[i];

            switch (state)
            {
                case State.Start:
                    if (char.IsLetter(c))
                    {
                        state = State.Identifier;
                        currentToken.Append(c);
                    }
                    else if (char.IsDigit(c))
                    {
                        state = State.Number;
                        currentToken.Append(c);
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        if (c == '\n') line++;
                    }
                    else if (c == '"')
                    {
                        state = State.String;
                    }
                    else if (c == '\'')
                    {
                        state = State.Character;
                    }
                    else if (c == '/' && i + 1 < buffer.Length && buffer[i + 1] == '/')
                    {
                        state = State.Comment;
                        i++; // Saltar el siguiente '/'
                    }
                    else if (i + 1 < buffer.Length && multiCharTokens.ContainsKey($"{c}{buffer[i + 1]}"))
                    {
                        tokens.Add(new Token(multiCharTokens[$"{c}{buffer[i + 1]}"], $"{c}{buffer[i + 1]}", string.Empty, line));
                        i++; // Saltar el siguiente carácter
                    }
                    else if (singleCharTokens.ContainsKey(c))
                    {
                        tokens.Add(new Token(singleCharTokens[c], c.ToString(), string.Empty, line));
                    }
                    else
                    {
                        throw new LexerException($"Token no reconocido: {c}");
                    }
                    i++;
                    break;

                case State.Number:
                    if (char.IsDigit(c))
                    {
                        currentToken.Append(c);
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString(), int.Parse(currentToken.ToString()), line));
                        currentToken.Clear();
                        state = State.Start;
                        continue; // Reprocesar el carácter actual
                    }
                    i++;
                    break;

                case State.Identifier:
                    if (char.IsLetterOrDigit(c))
                    {
                        currentToken.Append(c);
                    }
                    else
                    {
                        string id = currentToken.ToString();
                        tokens.Add(new Token(keywords.ContainsKey(id) ? keywords[id] : TokenType.ID, id, string.Empty, line));
                        currentToken.Clear();
                        state = State.Start;
                        continue; // Reprocesar el carácter actual
                    }
                    i++;
                    break;

                case State.String:
                    if (c != '"')
                    {
                        currentToken.Append(c);
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.STRING, currentToken.ToString(), currentToken.ToString(), line));
                        currentToken.Clear();
                        state = State.Start;
                    }
                    i++;
                    break;

                case State.Character:
                    if (c != '\'')
                    {
                        currentToken.Append(c);
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.CHARACTER, currentToken.ToString(), currentToken.ToString(), line));
                        currentToken.Clear();
                        state = State.Start;
                    }
                    i++;
                    break;

                case State.Comment:
                    if (c == '\n')
                    {
                        state = State.Start;
                        line++;
                    }
                    i++;
                    break;

                case State.Done:
                    break;
            }
        }

        if (currentToken.Length > 0)
        {
            switch (state)
            {
                case State.Number:
                    tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString(), int.Parse(currentToken.ToString()), line));
                    break;
                case State.Identifier:
                    string id = currentToken.ToString();
                    tokens.Add(new Token(keywords.ContainsKey(id) ? keywords[id] : TokenType.ID, id, string.Empty, line));
                    break;
                case State.String:
                    throw new LexerException("Cadena sin cerrar");
                case State.Character:
                    throw new LexerException("Carácter sin cerrar");
            }
        }

        tokens.Add(new Token(TokenType.EOF, "", string.Empty, line));
        return tokens;
    }

    private static string Preprocess(string texto)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;

        while (i < texto.Length)
        {
            char c = texto[i];

            if (c == '/' && i + 1 < texto.Length && texto[i + 1] == '/')
            {
                while (i < texto.Length && texto[i] != '\n')
                {
                    i++;
                }
            }
            else if (c == '/' && i + 1 < texto.Length && texto[i + 1] == '*')
            {
                while (i < texto.Length && !(texto[i] == '*' && texto[i + 1] == '/'))
                {
                    i++;
                }
                i += 2; // Saltar el cierre del comentario
            }
            else
            {
                result.Append(c);
            }
            i++;
        }

        return result.ToString();
    }
}
