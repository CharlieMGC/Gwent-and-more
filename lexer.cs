using System;
using System.Collections.Generic;

public class LexerException : Exception
{
    public LexerException(string message) : base(message) { }
}

public static class Lexer
{
    public static List<Token> Tokenizer(string texto)
    {
        List<Token> tokens = new List<Token>();
        int i = 0;

        while (i < texto.Length)
        {
            char c = texto[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }
            else if (char.IsDigit(c))
            {
                string number = "";
                while (i < texto.Length && char.IsDigit(texto[i]))
                {
                    number += texto[i];
                    i++;
                }
                tokens.Add(new Token(TokenType.NUMBER, number));
            }
            else if (c == '"')
            {
                string str = "";
                i++; // Saltar la comilla inicial
                while (i < texto.Length && texto[i] != '"')
                {
                    str += texto[i];
                    i++;
                }
                i++; // Saltar la comilla final
                tokens.Add(new Token(TokenType.STRING, str));
            }
            else if (c == '\'')
            {
                string character = "";
                i++; // Saltar la comilla inicial
                while (i < texto.Length && texto[i] != '\'')
                {
                    character += texto[i];
                    i++;
                }
                i++; // Saltar la comilla final
                tokens.Add(new Token(TokenType.CHARACTER, character));
            }
            else if (char.IsLetter(c))
            {
                string id = "";
                while (i < texto.Length && char.IsLetterOrDigit(texto[i]))
                {
                    id += texto[i];
                    i++;
                }
                tokens.Add(new Token(TokenType.ID, id));
            }
            else
            {
                switch (c)
                {
                    case '+':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.PLUS_ASSIGN, "+="));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '+')
                        {
                            tokens.Add(new Token(TokenType.INCREMENT, "++"));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.PLUS, c.ToString()));
                        }
                        break;
                    case '-':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.MINUS_ASSIGN, "-="));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '-')
                        {
                            tokens.Add(new Token(TokenType.DECREMENT, "--"));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.MINUS, c.ToString()));
                        }
                        break;
                    case '*':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.TIMES_ASSIGN, "*="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.TIMES, c.ToString()));
                        }
                        break;
                    case '/':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.DIVIDE_ASSIGN, "/="));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '/')
                        {
                            while (i < texto.Length && texto[i] != '\n')
                            {
                                i++;
                            }
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '*')
                        {
                            while (i < texto.Length && !(texto[i] == '*' && texto[i + 1] == '/'))
                            {
                                i++;
                            }
                            i += 2; // Saltar el cierre del comentario
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.DIVIDE, c.ToString()));
                        }
                        break;
                    case '%':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.MODULO_ASSIGN, "%="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.MODULO, c.ToString()));
                        }
                        break;
                    case '=':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.EQUAL, "=="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.ASSIGN, c.ToString()));
                        }
                        break;
                    case '!':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.NOT_EQUAL, "!="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.NOT, c.ToString()));
                        }
                        break;
                    case '<':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.LESS_EQUAL, "<="));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '<')
                        {
                            if (i + 2 < texto.Length && texto[i + 2] == '=')
                            {
                                tokens.Add(new Token(TokenType.SHIFT_LEFT_ASSIGN, "<<="));
                                i += 2;
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.SHIFT_LEFT, "<<"));
                                i++;
                            }
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.LESS_THAN, c.ToString()));
                        }
                        break;
                    case '>':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.GREATER_EQUAL, ">="));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '>')
                        {
                            if (i + 2 < texto.Length && texto[i + 2] == '=')
                            {
                                tokens.Add(new Token(TokenType.SHIFT_RIGHT_ASSIGN, ">>="));
                                i += 2;
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.SHIFT_RIGHT, ">>"));
                                i++;
                            }
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.GREATER_THAN, c.ToString()));
                        }
                        break;
                                        case '&':
                        if (i + 1 < texto.Length && texto[i + 1] == '&')
                        {
                            tokens.Add(new Token(TokenType.AND, "&&"));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.AND_ASSIGN, "&="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.BIT_AND, c.ToString()));
                        }
                        break;
                    case '|':
                        if (i + 1 < texto.Length && texto[i + 1] == '|')
                        {
                            tokens.Add(new Token(TokenType.OR, "||"));
                            i++;
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.OR_ASSIGN, "|="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.BIT_OR, c.ToString()));
                        }
                        break;
                    case '^':
                        if (i + 1 < texto.Length && texto[i + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.XOR_ASSIGN, "^="));
                            i++;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.BIT_XOR, c.ToString()));
                        }
                        break;
                    case '~':
                        tokens.Add(new Token(TokenType.BIT_NOT, c.ToString()));
                        break;
                    case '.':
                        tokens.Add(new Token(TokenType.DOT, c.ToString()));
                        break;
                    case '?':
                        tokens.Add(new Token(TokenType.QUESTION, c.ToString()));
                        break;
                    case '#':
                        tokens.Add(new Token(TokenType.HASH, c.ToString()));
                        break;
                    case '$':
                        tokens.Add(new Token(TokenType.DOLLAR, c.ToString()));
                        break;
                    case '\\':
                        tokens.Add(new Token(TokenType.BACKSLASH, c.ToString()));
                        break;
                    case '`':
                        tokens.Add(new Token(TokenType.BACKTICK, c.ToString()));
                        break;
                    case ',':
                        tokens.Add(new Token(TokenType.COMMA, c.ToString()));
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.SEMICOLON, c.ToString()));
                        break;
                    case ':':
                        tokens.Add(new Token(TokenType.COLON, c.ToString()));
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LPAREN, c.ToString()));
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RPAREN, c.ToString()));
                        break;
                    case '{':
                        tokens.Add(new Token(TokenType.LBRACE, c.ToString()));
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RBRACE, c.ToString()));
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LBRACKET, c.ToString()));
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RBRACKET, c.ToString()));
                        break;
                    case '\n':
                        tokens.Add(new Token(TokenType.NEWLINE, c.ToString()));
                        break;
                    default:
                        throw new LexerException($"Token no reconocido: {c}");
                }
                i++;
            }
        }
        return tokens;
    }
}
