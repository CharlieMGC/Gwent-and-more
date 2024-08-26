using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class Lexer
{
    private readonly List<(TokenType, Regex)> _tokenDefinitions;

    public Lexer()
    {
        _tokenDefinitions = new List<(TokenType, Regex)>
        {
            // Literales
            (TokenType.NUMBER, new Regex(@"\d+")),
            (TokenType.STRING, new Regex(@"""[^""]*""")),
            (TokenType.CHARACTER, new Regex(@"'[^']'")),

            // Identificadores
            (TokenType.ID, new Regex(@"[a-zA-Z_]\w*")),

            // Operadores
            (TokenType.PLUS, new Regex(@"\+")),
            (TokenType.MINUS, new Regex(@"-")),
            (TokenType.TIMES, new Regex(@"\*")),
            (TokenType.DIVIDE, new Regex(@"/")),
            (TokenType.MODULO, new Regex(@"%")),
            (TokenType.EQUAL, new Regex(@"==")),
            (TokenType.NOT_EQUAL, new Regex(@"!=")),
            (TokenType.LESS_THAN, new Regex(@"<")),
            (TokenType.GREATER_THAN, new Regex(@">")),
            (TokenType.LESS_EQUAL, new Regex(@"<=")),
            (TokenType.GREATER_EQUAL, new Regex(@">=")),
            (TokenType.AND, new Regex(@"&&")),
            (TokenType.OR, new Regex(@"\|\|")),
            (TokenType.NOT, new Regex(@"!")),
            (TokenType.ASSIGN, new Regex(@"=")),
            (TokenType.PLUS_ASSIGN, new Regex(@"\+=")),
            (TokenType.MINUS_ASSIGN, new Regex(@"-=")),
            (TokenType.TIMES_ASSIGN, new Regex(@"\*=")),
            (TokenType.DIVIDE_ASSIGN, new Regex(@"/=")),

            // Separadores
            (TokenType.COMMA, new Regex(@",")),
            (TokenType.SEMICOLON, new Regex(@";")),
            (TokenType.COLON, new Regex(@":")),

            // Corchetes
            (TokenType.LPAREN, new Regex(@"\(")),
            (TokenType.RPAREN, new Regex(@"\)")),
            (TokenType.LBRACE, new Regex(@"\{")),
            (TokenType.RBRACE, new Regex(@"\}")),
            (TokenType.LBRACKET, new Regex(@"\[")),
            (TokenType.RBRACKET, new Regex(@"\]")),

            // Comentarios
            (TokenType.COMMENT, new Regex(@"//.*|/\*[\s\S]*?\*/")),

            // Espacios en blanco
            (TokenType.WHITESPACE, new Regex(@"\s+")),

            // Palabras clave (ejemplo)
            (TokenType.KEYWORD, new Regex(@"\b(if|else|while|for|return)\b"))
        };
    }

    public List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        int position = 0;

        while (position < text.Length)
        {
            Token? match = null;

            foreach (var (type, regex) in _tokenDefinitions)
            {
                var matchResult = regex.Match(text, position);
                if (matchResult.Success && matchResult.Index == position)
                {
                    if (type != TokenType.WHITESPACE) // Ignorar espacios en blanco
                    {
                        match = new Token(type, matchResult.Value);
                        tokens.Add(match);
                    }
                    position += matchResult.Length;
                    break;
                }
            }

            if (match == null)
            {
                throw new Exception($"I can't understand what you wrote here dude {position}: {text[position]}");
            }
        }

        return tokens;
    }
}
