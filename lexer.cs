using System;
using System.Collections.Generic;

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
                        else
                        {
                            tokens.Add(new Token(TokenType.MINUS, c.ToString()));
                        }
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.TIMES, c.ToString()));
                        break;
                    case '/':
                        if (i + 1 < texto.Length && texto[i + 1] == '/')
                        {
                            string comment = "";
                            while (i < texto.Length && texto[i] != '\n')
                            {
                                comment += texto[i];
                                i++;
                            }
                            tokens.Add(new Token(TokenType.COMMENT, comment));
                        }
                        else if (i + 1 < texto.Length && texto[i + 1] == '*')
                        {
                            string comment = "";
                            while (i < texto.Length && !(texto[i] == '*' && texto[i + 1] == '/'))
                            {
                                comment += texto[i];
                                i++;
                            }
                            comment += "*/";
                            i += 2; // Saltar el cierre del comentario
                            tokens.Add(new Token(TokenType.COMMENT, comment));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.DIVIDE, c.ToString()));
                        }
                        break;
                    case '%':
                        tokens.Add(new Token(TokenType.MODULO, c.ToString()));
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
                        break;
                    case '|':
                        if (i + 1 < texto.Length && texto[i + 1] == '|')
                        {
                            tokens.Add(new Token(TokenType.OR, "||"));
                            i++;
                        }
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
                    default:
                        tokens.Add(new Token(TokenType.ID, c.ToString()));
                        break;
                }
                i++;
            }
        }

        return tokens;
    }
}
