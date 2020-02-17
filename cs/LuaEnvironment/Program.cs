using System;
using System.IO;
using static LuaDebugUtils;

namespace LuaEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                var data = File.ReadAllBytes(args[0]);
                var ls = new LuaState();
                ls.Register(nameof(Print).ToLower(), Print);
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
    }
}
