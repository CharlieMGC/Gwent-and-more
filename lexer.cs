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
            (TokenType.NUMBER, new Regex(@"\d+")),
            (TokenType.ID, new Regex(@"[a-zA-Z_]\w*")),
            (TokenType.PLUS, new Regex(@"\+")),
            (TokenType.MINUS, new Regex(@"-")),
            (TokenType.TIMES, new Regex(@"\*")),
            (TokenType.DIVIDE, new Regex(@"/")),
            (TokenType.LPAREN, new Regex(@"\(")),
            (TokenType.RPAREN, new Regex(@"\)")),
            (TokenType.WHITESPACE, new Regex(@"\s+")),
            (TokenType.LBRACE, new Regex(@"\{")),
            (TokenType.RBRACE, new Regex(@"\}")),
            (TokenType.COLON, new Regex(@":")),
            (TokenType.QUOTE, new Regex(@""""))
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
                int start = position;
                while (position < text.Length && !_tokenDefinitions.Any(td => td.Item2.IsMatch(text[position].ToString())))
                {
                    position++;
                }
                var unknownValue = text.Substring(start, position - start);
                var unknownToken = new Token(TokenType.UNKNOWN, unknownValue);
                tokens.Add(unknownToken);
            }
        }

        return tokens;
    }
}
