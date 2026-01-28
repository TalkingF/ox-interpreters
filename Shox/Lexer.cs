namespace Shox;

public class Lexer(string source)
{
    private readonly List<Token> _tokens = [];
    private int _current;
    private int _line = 1;
    private int _start;


    //iterates through source and tokenizes it
    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        //adds end of file token before returning
        _tokens.Add(new Token(TokenType.Eof, "", null, _line));
        return _tokens;
    }


    private bool IsAtEnd()
    {
        return _current >= source.Length;
    }


    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParen);
                break;
            case ')':
                AddToken(TokenType.RightParen);
                break;
            case '{':
                AddToken(TokenType.LeftBrace);
                break;
            case '}':
                AddToken(TokenType.RightBrace);
                break;
            case '+':
                AddToken(TokenType.Plus);
                break;
            case '-':
                AddToken(TokenType.Minus);
                break;
            case '*':
                AddToken(TokenType.Asterisk);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '.':
                AddToken(TokenType.Period);
                break;
            case ';':
                AddToken(TokenType.Semicolon);
                break;

            case '!':
                AddToken(Match('=')
                    ? TokenType.BangEqual
                    : TokenType.Bang);
                break;
            case '=':
                AddToken(Match('=')
                    ? TokenType.EqualEqual
                    : TokenType.Equal);
                break;
            case '<':
                AddToken(Match('=')
                    ? TokenType.LessEqual
                    : TokenType.Less);
                break;
            case '>':
                AddToken(Match('=')
                    ? TokenType.GreaterEqual
                    : TokenType.Greater);
                break;

            case '/':
                if (Match('/')) AdvanceToEndOfLine();
                else AddToken(TokenType.Slash);
                break;

            case '"':
                StringLiteral();
                break;

            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            default:

                if (char.IsDigit(c)) NumberLiteral();

                else if (IsAllowedInIdentifier(c)) IdentifierLiteral();

                Shox.Error(_line, "Unexpected character.");
                break;
        }
    }

    private char Advance()
    {
        return source[_current++];
    }

    // adds a token to the list, optionally adding a literal
    private void AddToken(TokenType type, object? literal = null)
    {
        var text = source[_start.._current];
        _tokens.Add(new Token(type, text, literal, _line));
    }

    //will consume next char in stream if and only if it matches expected
    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[_current] != expected) return false;

        _current++;
        return true;
    }

    //peeks at next char, does not consume
    private char Peek()
    {
        return IsAtEnd() ? '\0' : source[_current];
    }

    //Peeks at nth next char, equivalent to Peek() where n = 0
    private char PeekAhead(int n)
    {
        return _current + n > +source.Length ? '\0' : source[_current + n];
    }

    //advances lexer to end of line e.g. walking a comment
    private void AdvanceToEndOfLine()
    {
        while (Peek() != '\n' && !IsAtEnd()) Advance();
    }

    //attempts to generate a string literal
    private void StringLiteral()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Shox.Error(_line, "Unterminated string.");
            return;
        }

        Advance();

        var literal = source[(_start + 1)..(_current - 1)];
        AddToken(TokenType.String, literal);
    }

    //attempts to generate a number literal
    private void NumberLiteral()
    {
        //left side of digit
        while (char.IsDigit(Peek())) Advance();

        if (Peek() == '.' && char.IsDigit(PeekAhead(1)))
        {
            Advance();
            while (char.IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.Number,
            double.Parse(source[_start.._current]));
    }

    //checks if a char is of the allowed values in shox [A-Z], [a-z], [0-9], _
    private static bool IsAllowedInIdentifier(char c)
    {
        return char.IsAsciiLetterOrDigit(c) || c == '_';
    }

    //constructs an identifier which may or may not be a reserved word
    private void IdentifierLiteral()
    {
        while (IsAllowedInIdentifier(Peek())) Advance();

        var text = source[_start.._current];

        AddToken(Enum.TryParse<TokenType>(text, true, out var type)
            ? type
            : TokenType.Identifier);
    }
}