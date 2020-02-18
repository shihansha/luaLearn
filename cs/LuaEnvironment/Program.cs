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
                var data = File.ReadAllBytes(args[0]);
                var ls = new LuaState();
                ProgramLuaState = ls;
                ls.Register(nameof(Print).ToLower(), Print);
                ls.Register(nameof(GetMetatable).ToLower(), GetMetatable);
                ls.Register(nameof(SetMetatable).ToLower(), SetMetatable);
                ls.Load(data, "chunk", "b");
                ls.Call(0, 0);
            }
        }

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
    }
}
