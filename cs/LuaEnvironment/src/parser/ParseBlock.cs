using System;
using System.Collections.Generic;
using System.Text;
using LuaEnvironment.src.ast;

namespace LuaEnvironment.src.parser
{
    public static class Parser
    {
        public static Block ParseBlock(Lexer lexer)
        {
            return new Block
            {
                Stats = ParseStats(lexer),
                RetExps = ParseRetExps(lexer),
                LastLine = lexer.Line
            };
        }

        private static List<Exp> ParseRetExps(Lexer lexer)
        {
            if (lexer.LookAhead() != TokenType.KW_RETURN)
            {
                return null;
            }

            lexer.NextToken();
            switch (lexer.LookAhead())
            {
                case TokenType.EOF:
                case TokenType.KW_END:
                case TokenType.KW_ELSE:
                case TokenType.KW_ELSEIF:
                case TokenType.KW_UNTIL:
                    return new List<Exp>();
                case TokenType.SEP_SEMI:
                    lexer.NextToken();
                    return new List<Exp>();
                default:
                    var exps = ParseExpList(lexer);
                    if (lexer.LookAhead() == TokenType.SEP_SEMI)
                    {
                        lexer.NextToken();
                    }
                    return exps;
            }
        }

        private static List<Exp> ParseExpList(Lexer lexer)
        {
            var exps = new List<Exp>();
            exps.Add(ParseExp(lexer));
            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                exps.Add(ParseExp(lexer));
            }
            return exps;
        }

        private static List<Stat> ParseStats(Lexer lexer)
        {
            var stats = new List<Stat>();
            while (!IsReturnOrBlockEnd(lexer.LookAhead()))
            {
                Stat stat = ParseStat(lexer);
                if (!(stat is EmptyStat))
                {
                    stats.Add(stat);
                }
            }
            return stats;
        }

        private static Stat ParseStat(Lexer lexer)
        {
            return (lexer.LookAhead()) switch
            {
                TokenType.SEP_SEMI => ParseEmptyStat(lexer),
                TokenType.KW_BREAK => ParseBreakStat(lexer),
                TokenType.SEP_LABEL => ParseLabelStat(lexer),
                TokenType.KW_GOTO => ParseGotoStat(lexer),
                TokenType.KW_DO => ParseDoStat(lexer),
                TokenType.KW_WHILE => ParseWhileStat(lexer),
                TokenType.KW_REPEAT => ParseRepeatStat(lexer),
                TokenType.KW_IF => ParseIfStat(lexer),
                TokenType.KW_FOR => ParseForStat(lexer),
                TokenType.KW_FUNCTION => ParseFuncDefStat(lexer),
                TokenType.KW_LOCAL => ParseLocalAssignOrFuncDefStat(lexer),
                _ => ParseAssignOrFuncCallStat(lexer),
            };
        }

        private static Stat ParseAssignOrFuncCallStat(Lexer lexer)
        {
            var prefixExp = ParsePrefixExp(lexer);
            if (prefixExp is FuncCallExp fc)
            {
                return new FuncCallStat
                {
                    Args = fc.Args,
                    LastLine = fc.LastLine,
                    Line = fc.Line,
                    NameExp = fc.NameExp,
                    PrefixExp = fc.PrefixExp
                };
            }
            else
            {
                return ParseAssignStat(lexer, prefixExp);
            }
        }

        private static AssignStat ParseAssignStat(Lexer lexer, Exp var0)
        {
            var varList = FinishVarList(lexer, var0);
            lexer.NextTokenOfKind(TokenType.OP_ASSIGN);
            var expList = ParseExpList(lexer);
            var lastLine = lexer.Line;
            return new AssignStat
            {
                ExpList = expList,
                LastLine = lastLine,
                VarList = varList,
            };
        }

        private static List<Exp> FinishVarList(Lexer lexer, Exp var0)
        {
            var vars = new List<Exp>() { CheckVar(lexer, var0) };
            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                var exp = ParsePrefixExp(lexer);
                vars.Add(CheckVar(lexer, exp));
            }
            return vars;
        }

        private static Exp CheckVar(Lexer lexer, Exp exp)
        {
            switch (exp)
            {
                case NameExp _:
                case TableAccessExp _:
                    return exp;
            }
            lexer.NextTokenOfKind(TokenType.UNEXPECTED);
            throw new Exception("unreachable!");
        }

        private static Stat ParseForStat(Lexer lexer)
        {
            var (lineOfFor, _) = lexer.NextTokenOfKind(TokenType.KW_FOR);
            var (_, name) = lexer.NextIdentifier();
            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                return FinishForNumStat(lexer, lineOfFor, name);
            }
            else
            {
                return FinishForInStat(lexer, name);
            }
        }

        private static Stat FinishForInStat(Lexer lexer, string name0)
        {
            var nameList = FinishNameList(lexer, name0);
            lexer.NextTokenOfKind(TokenType.KW_IN);
            var expList = ParseExpList(lexer);
            var (lineOfDo, _) = lexer.NextTokenOfKind(TokenType.KW_DO);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.KW_END);
            return new ForInStat
            {
                Block = block,
                ExpList = expList,
                LineOfDo = lineOfDo,
                NameList = nameList,
            };
        }

        private static List<string> FinishNameList(Lexer lexer, string name0)
        {
            var names = new List<string>() { name0 };
            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                var (_, name) = lexer.NextIdentifier();
                names.Add(name);
            }
            return names;
        }

        private static Stat FinishForNumStat(Lexer lexer, int lineOfFor, string varName)
        {
            lexer.NextTokenOfKind(TokenType.OP_ASSIGN);
            var initExp = ParseExp(lexer);
            lexer.NextTokenOfKind(TokenType.SEP_COMMA);
            var limitExp = ParseExp(lexer);

            Exp stepExp;
            if (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                stepExp = ParseExp(lexer);
            }
            else
            {
                stepExp = new IntegerExp { Line = lexer.Line, Val = 1 };
            }

            var (lineOfDo, _) = lexer.NextTokenOfKind(TokenType.KW_DO);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.KW_END);

            return new ForNumStat
            {
                Block = block,
                InitExp = initExp,
                LimitExp = limitExp,
                LineOfDo = lineOfDo,
                LineOfFor = lineOfFor,
                StepExp = stepExp,
                VarName = varName,
            };
        }

        private static Exp ParseExp(Lexer lexer)
        {
            return ParseExp12(lexer);
        }

        private static Exp ParseExp12(Lexer lexer)
        {
            var exp = ParseExp11(lexer);
            while (lexer.LookAhead() == TokenType.OP_OR)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp11(lexer)
                };

                exp = OptimizeLocalOr(exp as BinopExp);
            }
            return exp;
        }

        private static Exp ParseExp11(Lexer lexer)
        {
            var exp = ParseExp10(lexer);
            while (lexer.LookAhead() == TokenType.OP_AND)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp10(lexer)
                };

                exp = OptimizeLocalAnd(exp as BinopExp);
            }
            return exp;
        }


        private static Exp ParseExp10(Lexer lexer)
        {
            var exp = ParseExp9(lexer);
            while (true)
            {
                var token = lexer.LookAhead();
                if (token == TokenType.OP_LT ||
                token == TokenType.OP_LE ||
                token == TokenType.OP_GT ||
                token == TokenType.OP_GE ||
                token == TokenType.OP_EQ ||
                token == TokenType.OP_NE)
                {
                    var (line, op, _) = lexer.NextToken();
                    exp = new BinopExp
                    {
                        Line = line,
                        Op = op,
                        Exp1 = exp,
                        Exp2 = ParseExp9(lexer)
                    };
                }
                else
                {
                    return exp;
                }
            }
        }

        private static Exp OptimizeLocalOr(BinopExp exp)
        {
            if (IsTrue(exp.Exp1))
            {
                return exp.Exp1;
            }
            if (IsFalse(exp.Exp1) && !IsVarargOrFuncCall(exp.Exp2))
            {
                return exp.Exp2;
            }
            return exp;
        }


        private static Exp OptimizeLocalAnd(BinopExp exp)
        {
            if (IsFalse(exp.Exp1))
            {
                return exp.Exp1;
            }
            if (IsTrue(exp.Exp1) && !IsVarargOrFuncCall(exp.Exp2))
            {
                return exp.Exp2;
            }
            return exp;
        }

        private static bool IsVarargOrFuncCall(Exp exp)
        {
            switch (exp)
            {
                case VarargExp _:
                case FuncCallExp _:
                    return true;
                default: return false;
            }
        }

        private static bool IsFalse(Exp exp)
        {
            switch (exp)
            {
                case FalseExp _:
                case NilExp _:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsTrue(Exp exp)
        {
            switch (exp)
            {
                case TrueExp _:
                case IntegerExp _:
                case FloatExp _:
                case StringExp _:
                    return true;
                default:
                    return false;
            }
        }

        private static Exp ParseExp9(Lexer lexer)
        {
            var exp = ParseExp8(lexer);
            while (lexer.LookAhead() == TokenType.OP_BOR)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp8(lexer)
                };

                exp = OptimizeBitwiseBinaryOp(exp as BinopExp);
            }
            return exp;
        }

        private static Exp ParseExp8(Lexer lexer)
        {
            var exp = ParseExp7(lexer);
            while (lexer.LookAhead() == TokenType.OP_BXOR)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp7(lexer)
                };

                exp = OptimizeBitwiseBinaryOp(exp as BinopExp);
            }
            return exp;
        }


        private static Exp ParseExp7(Lexer lexer)
        {
            var exp = ParseExp6(lexer);
            while (lexer.LookAhead() == TokenType.OP_BAND)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp6(lexer)
                };

                exp = OptimizeBitwiseBinaryOp(exp as BinopExp);
            }
            return exp;
        }


        private static Exp ParseExp6(Lexer lexer)
        {
            var exp = ParseExp5(lexer);
            while (true) 
            {
                var token = lexer.LookAhead();
                if (token == TokenType.OP_SHL ||
                token == TokenType.OP_SHR)
                {
                    var (line, op, _) = lexer.NextToken();
                    exp = new BinopExp
                    {
                        Line = line,
                        Op = op,
                        Exp1 = exp,
                        Exp2 = ParseExp5(lexer)
                    };

                    exp = OptimizeBitwiseBinaryOp(exp as BinopExp);
                }
                else
                {
                    return exp;
                }
            }
        }

        private static Exp OptimizeBitwiseBinaryOp(BinopExp exp)
        {
            if (CastToInt(exp, out long i) && CastToInt(exp, out long j))
            {
                switch (exp.Op)
                {
                    case TokenType.OP_BAND: return new IntegerExp { Line = exp.Line, Val = i & j };
                    case TokenType.OP_BOR: return new IntegerExp { Line = exp.Line, Val = i | j };
                    case TokenType.OP_BXOR: return new IntegerExp { Line = exp.Line, Val = i ^ j };
                    case TokenType.OP_SHL: return new IntegerExp { Line = exp.Line, Val = Number.ShiftLeft(i, j) };
                    case TokenType.OP_SHR: return new IntegerExp { Line = exp.Line, Val = Number.ShiftRight(i, j) };
                    default: break;
                }
            }
            return exp;
        }

        private static Exp ParseExp5(Lexer lexer)
        {
            var exp = ParseExp4(lexer);
            if (lexer.LookAhead() != TokenType.OP_CONCAT)
            {
                return exp;
            }

            int line = 0;
            var exps = new List<Exp>();
            while (lexer.LookAhead() == TokenType.OP_CONCAT)
            {
                (line, _, _) = lexer.NextToken();
                exps.Add(ParseExp4(lexer));
            }
            return new ConcatExp
            {
                Line = line,
                Exps = exps
            };
        }

        private static Exp ParseExp4(Lexer lexer)
        {
            var exp = ParseExp3(lexer);
            while (true)
            {
                var token = lexer.LookAhead();
                if (token == TokenType.OP_ADD ||
                    token == TokenType.OP_SUB)
                {
                    var (line, op, _) = lexer.NextToken();
                    exp = new BinopExp
                    {
                        Line = line,
                        Op = op,
                        Exp1 = exp,
                        Exp2 = ParseExp3(lexer)
                    };

                    exp = OptimizeArithBinaryOp(exp as BinopExp);
                }
                else
                {
                    return exp;
                }
            }
        }


        private static Exp ParseExp3(Lexer lexer)
        {
            var exp = ParseExp2(lexer);
            while (true)
            {
                var token = lexer.LookAhead();
                if (token == TokenType.OP_MUL ||
                    token == TokenType.OP_DIV ||
                    token == TokenType.OP_IDIV ||
                    token == TokenType.OP_MOD)
                {
                    var (line, op, _) = lexer.NextToken();
                    exp = new BinopExp
                    {
                        Line = line,
                        Op = op,
                        Exp1 = exp,
                        Exp2 = ParseExp2(lexer)
                    };

                    exp = OptimizeArithBinaryOp(exp as BinopExp);
                }
                else
                {
                    return exp;
                }
            }
        }

        private static Exp OptimizeArithBinaryOp(BinopExp exp)
        {
            if (exp.Exp1 is IntegerExp x && exp.Exp2 is IntegerExp y)
            {
                switch (exp.Op)
                {
                    case TokenType.OP_ADD: return new IntegerExp { Line = exp.Line, Val = x.Val + y.Val };
                    case TokenType.OP_SUB: return new IntegerExp { Line = exp.Line, Val = x.Val - y.Val };
                    case TokenType.OP_MUL: return new IntegerExp { Line = exp.Line, Val = x.Val * y.Val };
                    case TokenType.OP_IDIV: 
                        if (y.Val != 0)
                        {
                            return new IntegerExp { Line = exp.Line, Val = Number.FloorDiv(x.Val, y.Val) };
                        }
                        break;
                    case TokenType.OP_MOD: 
                        if (y.Val != 0)
                        {
                            return new IntegerExp { Line = exp.Line, Val = Number.Mod(x.Val, y.Val) };
                        }
                        break;
                    default: break;
                }
                if (CastToFloat(exp.Exp1, out double f) && CastToFloat(exp.Exp2, out double g))
                {
                    switch (exp.Op)
                    {
                        case TokenType.OP_ADD: return new FloatExp { Line = exp.Line, Val = f + g };
                        case TokenType.OP_SUB: return new FloatExp { Line = exp.Line, Val = f - g };
                        case TokenType.OP_MUL: return new FloatExp { Line = exp.Line, Val = f * g };
                        case TokenType.OP_DIV: return new FloatExp { Line = exp.Line, Val = f / g };
                        case TokenType.OP_IDIV: return new FloatExp { Line = exp.Line, Val = Number.FloorDiv(f, g) };
                        case TokenType.OP_MOD: return new FloatExp { Line = exp.Line, Val = Number.Mod(f, g) };
                        case TokenType.OP_POW: return new FloatExp { Line = exp.Line, Val = Math.Pow(f, g) };
                        default: break;
                    }
                }
            }
            return exp;
        }

        private static bool CastToFloat(Exp exp, out double result)
        {
            switch (exp)
            {
                case IntegerExp x: result = x.Val; return true;
                case FloatExp x: result = x.Val; return true;
                default: result = 0; return false;
            }
        }

        private static bool CastToInt(Exp exp, out Int64 result)
        {
            switch (exp)
            {
                case IntegerExp x: result = x.Val; return true;
                case FloatExp x: 
                    {
                        bool ret;
                        (result, ret) = Number.FloatToInteger(x.Val);
                        return ret;
                    }
                default: result = 0; return false;
            }
        }

        private static Exp ParseExp2(Lexer lexer)
        {
            var type = lexer.LookAhead();
            if (type == TokenType.OP_UNM ||
                type == TokenType.OP_BNOT ||
                type == TokenType.OP_LEN ||
                type == TokenType.OP_NOT)
            {
                var (line, op, _) = lexer.NextToken();
                var exp = new UnopExp
                {
                    Line = line,
                    Op = op,
                    Exp = ParseExp2(lexer)
                };
                return OptimizeUnaryExp(exp);
            }
            return ParseExp1(lexer);
        }

        private static Exp OptimizeUnaryExp(UnopExp exp)
        {
            switch (exp.Op)
            {
                case TokenType.OP_UNM: return OptimizeUnm(exp);
                case TokenType.OP_NOT: return OptimizeNot(exp);
                case TokenType.OP_BNOT: return OptimizeBnot(exp);
                default: return exp;
            }
        }

        private static Exp OptimizeBnot(UnopExp exp)
        {
            switch (exp.Exp)
            {
                case IntegerExp x: x.Val = ~x.Val; return x;
                default: return exp;
            }
        }

        private static Exp OptimizeNot(UnopExp exp)
        {
            switch (exp.Exp)
            {
                case FalseExp x: return new TrueExp { Line = x.Line };
                case TrueExp x: return new FalseExp { Line = x.Line };
                default: return exp;
            }
        }

        private static Exp OptimizeUnm(UnopExp exp)
        {
            switch (exp.Exp)
            {
                case IntegerExp x: x.Val = -x.Val; return x;
                case FloatExp x: x.Val = -x.Val; return x;
                default: return exp;
            }
        }

        private static Exp ParseExp1(Lexer lexer)
        {
            var exp = ParseExp0(lexer);
            if (lexer.LookAhead() == TokenType.OP_POW)
            {
                var (line, op, _) = lexer.NextToken();
                exp = new BinopExp
                {
                    Line = line,
                    Op = op,
                    Exp1 = exp,
                    Exp2 = ParseExp2(lexer)
                };
            }
            return OptimizePow(exp);
        }

        private static Exp OptimizePow(Exp exp)
        {
            if (exp is BinopExp binop)
            {
                if (binop.Op == TokenType.OP_POW)
                {
                    binop.Exp2 = OptimizePow(binop.Exp2);
                }
                return OptimizeArithBinaryOp(binop);
            }
            return exp;
        }

        private static Exp ParseExp0(Lexer lexer)
        {
            var token = lexer.LookAhead();
            if (token == TokenType.VARARG)
            {
                var (line, _, _) = lexer.NextToken();
                return new VarargExp { Line = line }; 
            }
            else if (token == TokenType.KW_NIL)
            {
                var (line, _, _) = lexer.NextToken();
                return new NilExp { Line = line };
            }
            else if (token == TokenType.KW_TRUE)
            {
                var (line, _, _) = lexer.NextToken();
                return new TrueExp { Line = line };
            }
            else if (token == TokenType.KW_FALSE)
            {
                var (line, _, _) = lexer.NextToken();
                return new FalseExp { Line = line };
            }
            else if (token == TokenType.STRING)
            {
                var (line, _, tk) = lexer.NextToken();
                return new StringExp { Line = line, Str = tk };
            }
            else if (token == TokenType.NUMBER)
            {
                return ParseNumberExp(lexer);
            }
            else if (token == TokenType.SEP_LCURLY)
            {
                return ParseTableConstructorExp(lexer);
            }
            else if (token == TokenType.KW_FUNCTION)
            {
                lexer.NextToken();
                return ParseFuncDefExp(lexer);
            }
            else
            {
                return ParsePrefixExp(lexer);
            }
        }

        private static Exp ParsePrefixExp(Lexer lexer)
        {
            Exp exp;
            if (lexer.LookAhead() == TokenType.IDENTIFIER)
            {
                var (line, name) = lexer.NextIdentifier();
                exp = new NameExp { Line = line, Name = name };
            }
            else
            {
                exp = ParseParensExp(lexer);
            }
            return FinishPrefixExp(lexer, exp);
        }

        private static Exp ParseParensExp(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.SEP_LPAREN);
            var exp = ParseExp(lexer);
            lexer.NextTokenOfKind(TokenType.SEP_RPAREN);

            if (exp is VarargExp || exp is FuncCallExp || exp is NameExp || exp is TableAccessExp)
            {
                return new ParensExp { Exp = exp };
            }
            return exp;
        }

        private static Exp FinishPrefixExp(Lexer lexer, Exp exp)
        {
            while (true)
            {
                switch (lexer.LookAhead())
                {
                    case TokenType.SEP_LBRACK:
                        {
                            lexer.NextToken();
                            var keyExp = ParseExp(lexer);
                            lexer.NextTokenOfKind(TokenType.SEP_RBRACK);
                            exp = new TableAccessExp { KeyExp = keyExp, LastLine = lexer.Line, PrefixExp = exp };
                        }
                        break;
                    case TokenType.SEP_DOT:
                        {
                            lexer.NextToken();
                            var (line, name) = lexer.NextIdentifier();
                            var keyExp = new StringExp { Line = line, Str = name };
                            exp = new TableAccessExp { KeyExp = keyExp, LastLine = line, PrefixExp = exp };
                        }
                        break;
                    case TokenType.SEP_COLON:
                    case TokenType.SEP_LPAREN:
                    case TokenType.SEP_LCURLY:
                    case TokenType.STRING:
                        {
                            exp = FinishFuncCallExp(lexer, exp);
                        }
                        break;
                    default:
                        return exp;
                }
            }
        }

        private static FuncCallExp FinishFuncCallExp(Lexer lexer, Exp prefixExp)
        {
            var nameExp = ParseNameExp(lexer);
            var line = lexer.Line;
            var args = ParseArgs(lexer);
            var lastLine = lexer.Line;
            return new FuncCallExp
            {
                Line = line,
                LastLine = lastLine,
                PrefixExp = prefixExp,
                NameExp = nameExp,
                Args = args
            };
        }

        private static List<Exp> ParseArgs(Lexer lexer)
        {
            var args = new List<Exp>();
            switch (lexer.LookAhead())
            {
                case TokenType.SEP_LPAREN:
                    {
                        lexer.NextToken();
                        if (lexer.LookAhead() != TokenType.SEP_RPAREN)
                        {
                            args = ParseExpList(lexer);
                        }
                        lexer.NextTokenOfKind(TokenType.SEP_RPAREN);
                    }
                    break;
                case TokenType.SEP_LCURLY:
                    {
                        args = new List<Exp>() { ParseTableConstructorExp(lexer) };
                    }
                    break;
                default:
                    {
                        var (line, str) = lexer.NextTokenOfKind(TokenType.STRING);
                        args = new List<Exp>() { new StringExp { Line = line, Str = str } };
                    }
                    break;
            }
            return args;
        }

        private static StringExp ParseNameExp(Lexer lexer)
        {
            if (lexer.LookAhead() == TokenType.SEP_COLON)
            {
                lexer.NextToken();
                var (line, name) = lexer.NextIdentifier();
                return new StringExp { Line = line, Str = name };
            }
            return null;
        }

        private static TableConstructorExp ParseTableConstructorExp(Lexer lexer)
        {
            int line = lexer.Line;
            lexer.NextTokenOfKind(TokenType.SEP_LCURLY);
            var (keyExps, valExps) = ParseFieldList(lexer);
            lexer.NextTokenOfKind(TokenType.SEP_RCURLY);
            int lastLine = lexer.Line;
            return new TableConstructorExp
            {
                KeyExps = keyExps,
                ValExps = valExps,
                Line = line,
                LastLine = lastLine,
            };
        }

        private static (List<Exp> ks, List<Exp> vs) ParseFieldList(Lexer lexer)
        {
            var ks = new List<Exp>();
            var vs = new List<Exp>();
            if (lexer.LookAhead() != TokenType.SEP_RCURLY)
            {
                var (k, v) = ParseField(lexer);
                ks.Add(k);
                vs.Add(v);
                while (IsFieldSep(lexer.LookAhead()))
                {
                    lexer.NextToken();
                    if (lexer.LookAhead() != TokenType.SEP_RCURLY)
                    {
                        (k, v) = ParseField(lexer);
                        ks.Add(k);
                        vs.Add(v);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return (ks, vs);
        }

        private static bool IsFieldSep(TokenType tokenKind)
        {
            return tokenKind == TokenType.SEP_COMMA || tokenKind == TokenType.SEP_SEMI;
        }

        private static (Exp k, Exp v) ParseField(Lexer lexer)
        {
            Exp k;
            Exp v;
            if (lexer.LookAhead() == TokenType.SEP_LBRACK)
            {
                lexer.NextToken();
                k = ParseExp(lexer);
                lexer.NextTokenOfKind(TokenType.SEP_RBRACK);
                lexer.NextTokenOfKind(TokenType.OP_ASSIGN);
                v = ParseExp(lexer);
                return (k, v);
            }

            var exp = ParseExp(lexer);
            if (exp is NameExp nameExp)
            {
                if (lexer.LookAhead() == TokenType.OP_ASSIGN)
                {
                    lexer.NextToken();
                    k = new StringExp { Line = lexer.Line, Str = nameExp.Name };
                    v = ParseExp(lexer);
                    return (k, v);
                }
            }

            return (null, exp);
        }

        private static Exp ParseNumberExp(Lexer lexer)
        {
            var (line, _, token) = lexer.NextToken();
            var (i, ok) = Number.ParseInteger(token);
            if (ok)
            {
                return new IntegerExp { Line = line, Val = i };
            }
            var (f, ok2) = Number.ParseFloat(token);
            if (ok2)
            {
                return new FloatExp { Line = line, Val = f };
            }
            else
            {
                throw new Exception("not a number: " + token);
            }
        }

        private static IfStat ParseIfStat(Lexer lexer)
        {
            var exps = new List<Exp>();
            var blocks = new List<Block>();

            lexer.NextTokenOfKind(TokenType.KW_IF);
            exps.Add(ParseExp(lexer));
            lexer.NextTokenOfKind(TokenType.KW_THEN);
            blocks.Add(ParseBlock(lexer));

            while (lexer.LookAhead() == TokenType.KW_ELSEIF)
            {
                lexer.NextToken();
                exps.Add(ParseExp(lexer));
                lexer.NextTokenOfKind(TokenType.KW_THEN);
                blocks.Add(ParseBlock(lexer));
            }

            if (lexer.LookAhead() == TokenType.KW_ELSE)
            {
                lexer.NextToken();
                exps.Add(new TrueExp { Line = lexer.Line });
                blocks.Add(ParseBlock(lexer));
            }

            lexer.NextTokenOfKind(TokenType.KW_END);
            return new IfStat { Blocks = blocks, Exps = exps };
        }

        private static Stat ParseLocalAssignOrFuncDefStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_LOCAL);
            if (lexer.LookAhead() == TokenType.KW_FUNCTION)
            {
                return FinishLocalFuncDefStat(lexer);
            }
            else
            {
                return FinishLocalVarDeclStat(lexer);
            }
        }

        private static LocalVarDeclStat FinishLocalVarDeclStat(Lexer lexer)
        {
            var (_, name0) = lexer.NextIdentifier();
            var nameList = FinishNameList(lexer, name0);
            List<Exp> expList = null;
            if (lexer.LookAhead() == TokenType.OP_ASSIGN)
            {
                lexer.NextToken();
                expList = ParseExpList(lexer);
            }
            var lastLine = lexer.Line;
            return new LocalVarDeclStat
            {
                ExpList = expList,
                LastLine = lastLine,
                NameList = nameList,
            };
        }

        private static LocalFuncDefStat FinishLocalFuncDefStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_FUNCTION);
            var (_, name) = lexer.NextIdentifier();
            var fdExp = ParseFuncDefExp(lexer);
            return new LocalFuncDefStat
            {
                Exp = fdExp,
                Name = name
            };
        }

        private static FuncDefExp ParseFuncDefExp(Lexer lexer)
        {
            int line = lexer.Line;
            lexer.NextTokenOfKind(TokenType.SEP_LPAREN);
            var (parList, isVararg) = ParseParList(lexer);
            lexer.NextTokenOfKind(TokenType.SEP_RPAREN);
            var block = ParseBlock(lexer);
            var (lastLine, _) = lexer.NextTokenOfKind(TokenType.KW_END);
            return new FuncDefExp
            {
                Line = line,
                LastLine = lastLine,
                ParList = parList,
                IsVararg = isVararg,
                Block = block
            };
        }

        private static (List<string> name, bool isVararg) ParseParList(Lexer lexer)
        {
            var names = new List<string>();
            bool isVararg = false;
            var token = lexer.LookAhead();
            if (token == TokenType.SEP_RPAREN) return (null, false);
            if (token == TokenType.VARARG) return (null, true);

            var (_, name) = lexer.NextIdentifier();
            names.Add(name);
            while (lexer.LookAhead() == TokenType.SEP_COMMA)
            {
                lexer.NextToken();
                if (lexer.LookAhead() == TokenType.IDENTIFIER)
                {
                    (_, name) = lexer.NextIdentifier();
                    names.Add(name);
                }
                else
                {
                    lexer.NextTokenOfKind(TokenType.VARARG);
                    isVararg = true;
                    break;
                }
            }

            return (names, isVararg);
        }

        private static AssignStat ParseFuncDefStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_FUNCTION);
            var (fnExp, hasColon) = ParseFuncName(lexer);
            var fdExp = ParseFuncDefExp(lexer);
            if (hasColon)
            {
                fdExp.ParList.Insert(0, "self");
            }
            return new AssignStat
            {
                LastLine = fdExp.Line,
                VarList = new List<Exp> { fnExp },
                ExpList = new List<Exp> { fdExp },
            };
        }

        private static (Exp exp, bool hasColon) ParseFuncName(Lexer lexer)
        {
            var (line, name) = lexer.NextIdentifier();
            Exp exp = new NameExp { Line = line, Name = name };
            bool hasColon = false;
            while (lexer.LookAhead() == TokenType.SEP_DOT)
            {
                lexer.NextToken();
                (line, name) = lexer.NextIdentifier();
                var idx = new StringExp { Line = line, Str = name };
                exp = new TableAccessExp { LastLine = line, KeyExp = idx, PrefixExp = exp };
            }
            if (lexer.LookAhead() == TokenType.SEP_COLON)
            {
                lexer.NextToken();
                (line, name) = lexer.NextIdentifier();
                var idx = new StringExp { Line = line, Str = name };
                exp = new TableAccessExp { LastLine = line, KeyExp = idx, PrefixExp = exp };
                hasColon = true;
            }
            return (exp, hasColon);
        }

        private static RepeatStat ParseRepeatStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_REPEAT);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.KW_UNTIL);
            var exp = ParseExp(lexer);
            return new RepeatStat { Block = block, Exp = exp };
        }

        private static WhileStat ParseWhileStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_WHILE);
            var exp = ParseExp(lexer);
            lexer.NextTokenOfKind(TokenType.KW_DO);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.KW_END);
            return new WhileStat { Block = block, Exp = exp };
        }

        private static DoStat ParseDoStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_DO);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.KW_END);
            return new DoStat { Block = block };
        }

        private static GotoStat ParseGotoStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_GOTO);
            var (_, name) = lexer.NextIdentifier();
            return new GotoStat { Name = name };
        }

        private static LabelStat ParseLabelStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.SEP_LABEL);
            var (_, name) = lexer.NextIdentifier();
            lexer.NextTokenOfKind(TokenType.SEP_LABEL);
            return new LabelStat { Name = name };
        }

        private static BreakStat ParseBreakStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.KW_BREAK);
            return new BreakStat { Line = lexer.Line };
        }

        private static EmptyStat ParseEmptyStat(Lexer lexer)
        {
            lexer.NextTokenOfKind(TokenType.SEP_SEMI);
            return new EmptyStat();
        }

        private static bool IsReturnOrBlockEnd(TokenType tokenKind)
        {
            switch (tokenKind)
            {
                case TokenType.KW_RETURN:
                case TokenType.EOF:
                case TokenType.KW_END:
                case TokenType.KW_ELSE:
                case TokenType.KW_ELSEIF:
                case TokenType.KW_UNTIL:
                    return true;
                default:
                    return false;
            }
        }

        public static Block Parse(string chunk, string chunkName)
        {
            var lexer = new Lexer(chunk, chunkName);
            var block = ParseBlock(lexer);
            lexer.NextTokenOfKind(TokenType.EOF);
            return block;
        }
    }
}
