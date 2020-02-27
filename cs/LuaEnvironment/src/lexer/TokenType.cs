using System.Collections.Generic;

public enum TokenType
{
    EOF = 0, // end-of-file
    VARARG, // ...
    SEP_SEMI, // ;
    SEP_COMMA, // ,
    SEP_DOT, // .
    SEP_COLON, // :
    SEP_LABEL, // ::
    SEP_LPAREN, // (
    SEP_RPAREN, // )
    SEP_LBRACK, // [
    SEP_RBRACK, // ]
    SEP_LCURLY, // {
    SEP_RCURLY, // }
    OP_ASSIGN, // =
    OP_MINUS, // - (sub or unm)
    OP_WAVE, // ~ (bnot or bxor)
    OP_ADD, // +
    OP_MUL, // *
    OP_DIV, // /
    OP_IDIV, // //
    OP_POW, // ^
    OP_MOD, // %
    OP_BAND, // &
    OP_BOR, // |
    OP_SHR, // >>
    OP_SHL, // <<
    OP_CONCAT, // ..
    OP_LT, // <
    OP_LE, // <=
    OP_GT, // >
    OP_GE, // >=
    OP_EQ, // ==
    OP_NE, // ~=
    OP_LEN, // #
    OP_AND, // and
    OP_OR, // or
    OP_NOT, // not
    KW_BREAK, // break
    KW_DO, // do
    KW_ELSE, // else
    KW_ELSEIF, // elseif
    KW_END, // end
    KW_FALSE, // false
    KW_FOR, // for
    KW_FUNCTION, // function
    KW_GOTO, // goto
    KW_IF, // if
    KW_IN, // in
    KW_LOCAL, // local
    KW_NIL, // nil
    KW_REPEAT, // repeat
    KW_RETURN, // return
    KW_THEN, // then
    KW_TRUE, // true
    KW_UNTIL, // until
    KW_WHILE, // while
    IDENTIFIER, // identifier
    NUMBER, // number
    STRING, // string literal
    OP_UNM = OP_MINUS, // unary minus
    OP_SUB = OP_MINUS,
    OP_BNOT = OP_WAVE,
    OP_BXOR = OP_WAVE,

    UNEXPECTED = -1
}

public static class Keywords
{
    public static readonly Dictionary<string, TokenType> Map = new Dictionary<string, TokenType>
    {
        { "and", TokenType.OP_AND },
        { "break", TokenType.KW_BREAK },
        { "do", TokenType.KW_DO },
        { "else", TokenType.KW_ELSE },
        { "elseif", TokenType.KW_ELSEIF },
        { "end", TokenType.KW_END },
        { "false", TokenType.KW_FALSE },
        { "for", TokenType.KW_FOR },
        { "function", TokenType.KW_FUNCTION },
        { "goto", TokenType.KW_GOTO },
        { "if", TokenType.KW_IF },
        { "in", TokenType.KW_IN },
        { "local", TokenType.KW_LOCAL },
        { "nil", TokenType.KW_NIL },
        { "not", TokenType.OP_NOT },
        { "or", TokenType.OP_OR },
        { "repeat", TokenType.KW_REPEAT },
        { "return", TokenType.KW_RETURN },
        { "then", TokenType.KW_THEN },
        { "true", TokenType.KW_TRUE },
        { "until", TokenType.KW_UNTIL },
        { "while", TokenType.KW_WHILE },
    };
}
