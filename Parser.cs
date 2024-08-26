using System;
using System.Collections.Generic;

public class Parser
{
    private List<Token> tokens;
    private int position;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.position = 0;
    }

    private Token? CurrentToken => position < tokens.Count ? tokens[position] : null;

    private void Advance() => position++;

    private bool Match(TokenType type)
    {
        if (CurrentToken != null && CurrentToken.Type == type)
        {
            Advance();
            return true;
        }
        return false;
    }

    public void Parse()
    {
        while (CurrentToken != null)
        {
            if (Match(TokenType.KEYWORD))
            {
                Console.WriteLine("Keyword: " + CurrentToken.Value);
            }
            else if (Match(TokenType.ID))
            {
                Console.WriteLine("Identifier: " + CurrentToken.Value);
            }
            else if (Match(TokenType.NUMBER))
            {
                Console.WriteLine("Number: " + CurrentToken.Value);
            }
            else
            {
                throw new Exception("Unexpected token: " + CurrentToken.Value);
            }
        }
    }
}
