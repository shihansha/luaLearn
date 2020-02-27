using LuaEnvironment.src.parser;
using System;
using System.IO;
using static LuaDebugUtils;

namespace LuaEnvironment
{
    public class Program
    {
        public static LuaState ProgramLuaState;
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                //var data = File.ReadAllBytes(args[0]);
                //var ls = new LuaState();
                //ProgramLuaState = ls;
                //ls.Register(nameof(Print).ToLower(), Print);
                //ls.Register(nameof(GetMetatable).ToLower(), GetMetatable);
                //ls.Register(nameof(SetMetatable).ToLower(), SetMetatable);
                //ls.Register(nameof(Next).ToLower(), Next);
                //ls.Register(nameof(Pairs).ToLower(), Pairs);
                //ls.Register(nameof(IPairs).ToLower(), IPairs);
                //ls.Register(nameof(Error).ToLower(), Error);
                //ls.Register(nameof(PCall).ToLower(), PCall);
                //ls.Load(data, "chunk", "b");
                //ls.Call(0, 0);

                string data = File.ReadAllText(args[0]);
                //TestLexer(data, args[0]);
                TestParser(data, args[0]);  
            }
        }

        #region VM
        private static int Print(ILuaState ls)
        {
            int nArgs = ls.GetTop();
            for (int i = 1; i <= nArgs; i++)
            {
                if (ls.IsBoolean(i))
                {
                    Console.Write(ls.ToBoolean(i));
                }
                else if (ls.IsString(i))
                {
                    Console.Write(ls.ToString(i));
                }
                else
                {
                    Console.Write(ls.TypeName(ls.Type(i)));
                }
                if (i < nArgs)
                {
                    Console.Write("\t");
                }
            }
            Console.WriteLine();
            return 0;
        }

        private static int GetMetatable(ILuaState ls)
        {
            if (!ls.GetMetatable(1))
            {
                ls.PushNil();
            }
            return 1;
        }

        private static int SetMetatable(ILuaState ls)
        {
            ls.SetMetatable(1);
            return 1;
        }

        private static int Next(ILuaState ls)
        {
            ls.SetTop(2);
            if (ls.Next(1))
            {
                return 2;
            }
            else
            {
                ls.PushNil();
                return 1;
            }
        }

        private static int Pairs(ILuaState ls)
        {
            ls.PushCSharpFunction(Next);
            ls.PushValue(1);
            ls.PushNil();
            return 3;
        }

        private static int IPairsAux(ILuaState ls)
        {
            var i = ls.ToInteger(2) + 1;
            ls.PushInteger(i);
            if (ls.GetI(1, i) == LuaType.Nil)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private static int IPairs(ILuaState ls)
        {
            ls.PushCSharpFunction(IPairsAux);
            ls.PushValue(1);
            ls.PushInteger(0);
            return 3;
        }

        private static int Error(ILuaState ls)
        {
            return ls.Error();
        }

        private static int PCall(ILuaState ls)
        {
            int nArgs = ls.GetTop() - 1;
            var status = ls.PCall(nArgs, -1, 0);
            ls.PushBoolean(status == ErrState.Ok);
            ls.Insert(1);
            return ls.GetTop();
        }
        #endregion
        #region Lex
        private static void TestLexer(string chunk, string chunkName)
        {
            Lexer lexer = new Lexer(chunk, chunkName);
            while (true)
            {
                var (line, kind, token) = lexer.NextToken();
                Console.WriteLine($"[{line:D2}] [{KindToCategory(kind),-10}]: {token}");
                if (kind == TokenType.EOF)
                {
                    break;
                }
            }
        }

        private static void TestParser(string chunk, string chunkName)
        {
            var ast = Parser.Parse(chunk, chunkName);
            string b = Newtonsoft.Json.JsonConvert.SerializeObject(ast, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(b);
        }

        private static string KindToCategory(TokenType kind)
        {
            if (kind < TokenType.SEP_SEMI) return "other";
            else if (kind <= TokenType.SEP_RCURLY) return "separator";
            else if (kind <= TokenType.OP_NOT) return "operator";
            else if (kind <= TokenType.KW_WHILE) return "keyword";
            else if (kind == TokenType.IDENTIFIER) return "identifier";
            else if (kind == TokenType.NUMBER) return "number";
            else if (kind == TokenType.STRING) return "string";
            else return "other";
        }
        #endregion
    }
}
