using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class Lexer
{
    private string chunk; // 源代码
    private readonly string chunkName; // 源文件名
    private string nextToken;
    private TokenType nextTokenKind;
    private int nextTokenLine;
    
    private static readonly Regex reOpeningLongBracket = new Regex(@"^\[=*\[");
    private static readonly Regex reNewLine = new Regex(@"\r\n|\n\r|\n|\r");
    private static readonly Regex reShortStr = new Regex(@"(?s)(^'(\\\\|\\'|\\(\r\n|\n\r|\n|\r)|\\z\s*|[^'(\r\n|\n\r|\n|\r)])*')|(^""(\\\\|\\""|\\(\r\n|\n\r|\n|\r)|\\z\s*|[^""(\r\n|\n\r|\n|\r)])*"")");
    private static readonly Regex reDecEscapeSeq = new Regex(@"^\\\d{1,3}");
    private static readonly Regex reHexEscapeSeq = new Regex(@"^\\x[0-9a-fA-F]{2}");
    private static readonly Regex reUnicodeEscapeSeq = new Regex(@"^\\u\{[0-9a-fA-F]+\}");
    private static readonly Regex reNumber = new Regex(@"^0[xX][0-9a-fA-F]*(\.[0-9a-fA-F]*)?([pP][+\-]?[0-9]+)?|^[0-9]*(\.[0-9]*)?([eE][+\-]?[0-9]+)?");
    private static readonly Regex reIdentifier = new Regex(@"^[_\d\w]+");

    public Lexer(string chunk, string chunkName)
    {
        this.chunk = chunk;
        this.chunkName = chunkName;
        Line = 1;
    }

    private int line;
    public int Line
    {
        get => line;
        private set
        {
            if (value != line)
            {
                // Console.WriteLine("line add: " + (value - line));
                line = value;
            }
        }
    }

    public TokenType LookAhead()
    {
        var val = LookAheadInside();
        // Console.WriteLine("look ahead: " + val);
        return val;
    }

    private TokenType LookAheadInside()
    {
        if (nextTokenLine > 0)
        {
            return nextTokenKind;
        }
        int currentLine = this.Line;
        var (line, kind, token) = NextToken();
        this.Line = currentLine;
        nextTokenLine = line;
        nextTokenKind = kind;
        nextToken = token;
        return kind;
    }

    public (int line, string token) NextIdentifier()
    {
        return NextTokenOfKind(TokenType.IDENTIFIER);
    }

    public (int line, TokenType kind, string token) NextToken()
    {
        var val = NextTokenInside();
        // Console.WriteLine("next token: " + val.kind);
        return val;
    }

    private (int line, TokenType kind, string token) NextTokenInside()
    {
        if (nextTokenLine > 0)
        {
            int line = nextTokenLine;
            TokenType kind = nextTokenKind;
            string token = nextToken;
            this.Line = nextTokenLine;
            nextTokenLine = 0;
            return (line, kind, token);
        }

        SkipWhiteSpaces();
        if (chunk.Length == 0)
        {
            return (Line, TokenType.EOF, "EOF");
        }

        switch (chunk[0])
        {
            case ';': Next(1); return (Line, TokenType.SEP_SEMI, ";");
            case ',': Next(1); return (Line, TokenType.SEP_COMMA, ",");
            case '(': Next(1); return (Line, TokenType.SEP_LPAREN, "(");
            case ')': Next(1); return (Line, TokenType.SEP_RPAREN, ")");
            case ']': Next(1); return (Line, TokenType.SEP_RBRACK, "]");
            case '{': Next(1); return (Line, TokenType.SEP_LCURLY, "{");
            case '}': Next(1); return (Line, TokenType.SEP_RCURLY, "}");
            case '+': Next(1); return (Line, TokenType.OP_ADD, "+");
            case '-': Next(1); return (Line, TokenType.OP_MINUS, "-");
            case '*': Next(1); return (Line, TokenType.OP_MUL, "*");
            case '^': Next(1); return (Line, TokenType.OP_POW, "^");
            case '%': Next(1); return (Line, TokenType.OP_MOD, "%");
            case '&': Next(1); return (Line, TokenType.OP_BAND, "&");
            case '|': Next(1); return (Line, TokenType.OP_BOR, "|");
            case '#': Next(1); return (Line, TokenType.OP_LEN, "#");
            case ':':
                {
                    if (Test("::"))
                    {
                        Next(2);
                        return (Line, TokenType.SEP_LABEL, "::");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.SEP_COLON, ":");
                    }
                }
            case '/':
                {
                    if (Test("//"))
                    {
                        Next(2);
                        return (Line, TokenType.OP_IDIV, "//");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.OP_DIV, "/");
                    }
                }
            case '~':
                {
                    if (Test("~="))
                    {
                        Next(2);
                        return (Line, TokenType.OP_NE, "~=");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.OP_WAVE, "~");
                    }
                }
            case '=':
                {
                    if (Test("=="))
                    {
                        Next(2);
                        return (Line, TokenType.OP_EQ, "==");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.OP_ASSIGN, "=");
                    }
                }
            case '<':
                {
                    if (Test("<<"))
                    {
                        Next(2);
                        return (Line, TokenType.OP_SHL, "<<");
                    }
                    else if (Test("<="))
                    {
                        Next(2);
                        return (Line, TokenType.OP_LE, "<=");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.OP_LT, "<");
                    }
                }
            case '>':
                {
                    if (Test(">>"))
                    {
                        Next(2);
                        return (Line, TokenType.OP_SHR, ">>");
                    }
                    else if (Test(">="))
                    {
                        Next(2);
                        return (Line, TokenType.OP_GE, ">=");
                    }
                    else
                    {
                        Next(1);
                        return (Line, TokenType.OP_GT, ">");
                    }
                }
            case '.':
                {
                    if (Test("..."))
                    {
                        Next(3);
                        return (Line, TokenType.VARARG, "...");
                    }
                    else if (Test(".."))
                    {
                        Next(2);
                        return (Line, TokenType.OP_CONCAT, "..");
                    }
                    else if (chunk.Length == 1 || !IsDigit(chunk[1]))
                    {
                        Next(1);
                        return (Line, TokenType.SEP_DOT, ".");
                    }
                    break;
                }
            case '[':
                if (Test("[[") || Test("[="))
                {
                    string token = ScanLongString();
                    return (Line, TokenType.STRING, token);
                }
                else
                {
                    Next(1);
                    return (Line, TokenType.SEP_LBRACK, "[");
                }
            case '\'':
            case '"':
                {
                    string token = ScanShortString();
                    return (Line, TokenType.STRING, token);
                }
        }

        char c = chunk[0];
        if (c == '.' || IsDigit(c))
        {
            string token = ScanNumber();
            return (Line, TokenType.NUMBER, token);
        }
        if (c == '_' || IsLatter(c))
        {
            string token = ScanIdentifier();
            if (Keywords.Map.TryGetValue(token, out TokenType kind))
            {
                return (Line, kind, token);
            }
            else
            {
                return (Line, TokenType.IDENTIFIER, token);
            }
        }

        Error("unexpected symbol near {0}", c);

        throw new Exception("unreachable!");

    }

    public (int line, string token) NextTokenOfKind(TokenType kind)
    {
        var next = NextToken();
        if (next.kind != kind)
        {
            Error("syntax error near '{0}'", next.token);
        }
        return (next.line, next.token);
    }

    private string ScanIdentifier()
    {
        return Scan(reIdentifier);
    }

    private bool IsLatter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private string ScanNumber()
    {
        return Scan(reNumber);
    }

    private string Scan(Regex regex)
    {
        var match = regex.Match(chunk);
        if (match.Success)
        {
            string token = match.Value;
            Next(token.Length);
            return token;
        }
        throw new Exception("unreachable!");
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private string ScanShortString()
    {
        var match = reShortStr.Match(chunk);
        if (match.Success)
        {
            string str = match.Value;
            Next(str.Length);
            str = str[1..(str.Length - 1)];
            if (str.IndexOf('\\') >= 0)
            {
                int newLineCount = reNewLine.Matches(str).Count;
                Line += newLineCount;
                str = Escape(str);
            }
            return str;
        }
        Error("unfinished string");
        return "";
    }

    private string Escape(string str)
    {
        
        StringBuilder buf = new StringBuilder();
        while (str.Length > 0)
        {
            if (str[0] != '\\')
            {
                buf.Append(str[0]);
                str = str[1..];
                continue;
            }
            if (str.Length == 1)
            {
                Error("unfinished string");
            }
            switch (str[1])
            {
                case 'a': buf.Append('\a'); str = str[2..]; continue;
                case 'b': buf.Append('\b'); str = str[2..]; continue;
                case 'f': buf.Append('\f'); str = str[2..]; continue;
                case 'n': buf.Append('\n'); str = str[2..]; continue;
                case '\n':
                case '\r':
                    {
                        buf.Append('\n');
                        if (str.Length >= 3 && (str[2] == '\r' || str[2] == '\n'))
                        {
                            str = str[3..];
                        }
                        else
                        {
                            str = str[2..];
                        }
                        continue;
                    }
                case 'r': buf.Append('\r'); str = str[2..]; continue;
                case 't': buf.Append('\t'); str = str[2..]; continue;
                case 'v': buf.Append('\v'); str = str[2..]; continue;
                case '"': buf.Append('"'); str = str[2..]; continue;
                case '\'': buf.Append('\''); str = str[2..]; continue;
                case '\\': buf.Append('\\'); str = str[2..]; continue;
                case char c when IsDigit(c):
                    {
                        var match = reDecEscapeSeq.Match(str);
                        if (match.Success)
                        {
                            string found = match.Value;
                            int d = int.Parse(found[1..]);
                            if (d <= 0xff)
                            {
                                buf.Append((char)d);
                                str = str[found.Length..];
                                continue;
                            }
                            Error("decimal escape too large near '{0}'", found);
                        }
                    }
                    break;
                case 'x': // \xxx
                    {
                        var match = reHexEscapeSeq.Match(str);
                        if (match.Success)
                        {
                            string found = match.Value;
                            int d = int.Parse(found[2..], System.Globalization.NumberStyles.HexNumber);
                            buf.Append((char)d);
                            str = str[found.Length..];
                            continue;
                        }
                    }
                    break;
                case 'u': // \u{xxx}
                    {
                        var match = reUnicodeEscapeSeq.Match(str);
                        if (match.Success)
                        {
                            string found = match.Value;
                            if (int.TryParse(found[3..(found.Length - 1)], System.Globalization.NumberStyles.HexNumber, null, out int d) &&
                                d <= 0x10ffff)
                            {
                                buf.Append((char)d);
                                str = str[found.Length..];
                                continue;
                            }
                            Error("UTF-8 value too large near '{0}'", found);
                        }
                    }
                    break;
                case 'z':
                    {
                        str = str[2..];
                        while (str.Length > 0 && IsWhiteSpace(str[0]))
                        {
                            str = str[1..];
                        }
                        continue;
                    }
                default:
                    Error("invalid escape sequence near '\\{0}'", str[1]);
                    break;
            }
        }
        return buf.ToString();
    }

    private string ScanLongString()
    {
        var openingRegex = reOpeningLongBracket.Match(chunk);
        if (!openingRegex.Success)
        {
            Error($"invalid long string delimiter near '{chunk[0..2]}'");
        }

        string openingLongBracket = openingRegex.Value;
        string closingLongBracket = openingLongBracket.Replace("[", "]");
        int closingLongBracketIdx = chunk.IndexOf(closingLongBracket);
        if (closingLongBracketIdx < 0)
        {
            Error("unfinished long string or comment");
        }

        string str = chunk[openingLongBracket.Length..closingLongBracketIdx];
        Next(closingLongBracketIdx + closingLongBracket.Length);

        str = reNewLine.Replace(str, "\n");
        Line += str.Count(a => a == '\n');
        if (str.Length > 0 && str[0] == '\n')
        {
            str = str[1..];
        }
        return str;
    }

    private void Error(string f, params object[] a)
    {
        string err = string.Format(f, a);
        err = $"{chunkName}:{Line}: {err}";
        throw new LexerException(err);
    }

    private void Next(int n)
    {
        chunk = chunk[n..];
    }

    private void SkipWhiteSpaces()
    {
        while (chunk.Length > 0)
        {
            if (Test("--"))
            {
                SkipComment();
            }
            else if (Test("\r\n") || Test("\n\r"))
            {
                Next(2);
                Line += 1;
            }
            else if (IsNewLine(chunk[0]))
            {
                Next(1);
                Line += 1;
            }
            else if (IsWhiteSpace(chunk[0]))
            {
                Next(1);
            }
            else
            {
                break;
            }
        }
    }

    private void SkipComment()
    {
        Next(2); // skip --
        if (Test("[")) // long comment ?
        {
            if (reOpeningLongBracket.Match(chunk).Success)
            {
                ScanLongString();
                return;
            }
        }

        // short comment
        while (chunk.Length > 0 && !IsNewLine(chunk[0]))
        {
            Next(1);
        }
    }

    private bool IsWhiteSpace(char c)
    {
        if (c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r' || c == ' ')
        {
            return true;
        }
        return false;
    }

    private bool IsNewLine(char c)
    {
        return c == '\r' || c == '\n';
    }

    private bool Test(string s)
    {
        return chunk.StartsWith(s);
    }
}
