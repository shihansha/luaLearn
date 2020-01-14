using System;
using System.IO;
using static LuaDebugUtils;

namespace LuaEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            // if (args.Length >= 1)
            // {
            //     var data = File.ReadAllBytes(args[0]);
            //     Console.WriteLine("data length: " + data.Length);
            //     Prototype proto = Prototype.Undump(data);
            //     Console.WriteLine(proto);
            // }
            LuaState ls = LuaState.New();
            ls.PushBoolean(true); PrintStack(ls);
            ls.PushInteger(10); PrintStack(ls);
            ls.PushNil(); PrintStack(ls);
            ls.PushString("Hello"); PrintStack(ls);
            ls.PushValue(-4); PrintStack(ls);
            ls.Replace(3); PrintStack(ls);
            ls.SetTop(6); PrintStack(ls);
            ls.Remove(-3); PrintStack(ls);
            ls.SetTop(-5); PrintStack(ls);
        }
    }
}
