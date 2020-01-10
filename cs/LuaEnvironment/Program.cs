using System;
using System.IO;

namespace LuaEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in args)
            {
                Console.WriteLine("path: " + item);
            }

            if (args.Length >= 1)
            {
                var data = File.ReadAllBytes(args[0]);
                Console.WriteLine("data length: " + data.Length);
                Prototype proto = Prototype.Undump(data);
                Console.WriteLine(proto);
            }
        }
    }
}
