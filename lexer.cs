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

    public static List<Token> Tokenizer(string texto)
    {
        texto = Preprocess(texto);
        List<Token> tokens = new List<Token>();
        int i = 0;
        State state = State.Start;
        StringBuilder currentToken = new StringBuilder();
        char[] buffer = texto.ToCharArray();

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
                        // Ignorar espacios en blanco
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
                    else if (singleCharTokens.ContainsKey(c))
                    {
                        tokens.Add(new Token(singleCharTokens[c], c.ToString()));
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
                        tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString()));
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
                        tokens.Add(new Token(keywords.ContainsKey(id) ? keywords[id] : TokenType.ID, id));
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
                        tokens.Add(new Token(TokenType.STRING, currentToken.ToString()));
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
                        tokens.Add(new Token(TokenType.CHARACTER, currentToken.ToString()));
                        currentToken.Clear();
                        state = State.Start;
                    }
                    i++;
                    break;

                case State.Comment:
                    if (c == '\n')
                    {
                        state = State.Start;
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
                    tokens.Add(new Token(TokenType.NUMBER, currentToken.ToString()));
                    break;
                case State.Identifier:
                    string id = currentToken.ToString();
                    tokens.Add(new Token(keywords.ContainsKey(id) ? keywords[id] : TokenType.ID, id));
                    break;
                case State.String:
                    throw new LexerException("Cadena sin cerrar");
                case State.Character:
                    throw new LexerException("Carácter sin cerrar");
            }
        }

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
