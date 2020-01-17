using System;
using System.IO;
using static LuaDebugUtils;

namespace LuaEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            //// ch3 debug
            // if (args.Length >= 1)
            // {
            //     var data = File.ReadAllBytes(args[0]);
            //     Console.WriteLine("data length: " + data.Length);
            //     Prototype proto = Prototype.Undump(data);
            //     Console.WriteLine(proto);
            // }

            //// ch4 debug
            // LuaState ls = LuaState.New();
            // ls.PushBoolean(true); PrintStack(ls);
            // ls.PushInteger(10); PrintStack(ls);
            // ls.PushNil(); PrintStack(ls);
            // ls.PushString("Hello"); PrintStack(ls);
            // ls.PushValue(-4); PrintStack(ls);
            // ls.Replace(3); PrintStack(ls);
            // ls.SetTop(6); PrintStack(ls);
            // ls.Remove(-3); PrintStack(ls);
            // ls.SetTop(-5); PrintStack(ls);

            //// ch5 debug
            // var ls = LuaState.New();
            // ls.PushInteger(1);
            // ls.PushString("2.0");
            // ls.PushString("3.0");
            // ls.PushNumber(4.0);
            // PrintStack(ls);

            // ls.Arith(ArithOp.Add); PrintStack(ls);
            // ls.Arith(ArithOp.Bnot); PrintStack(ls);
            // ls.Len(2); PrintStack(ls);
            // ls.Concat(3); PrintStack(ls);
            // ls.PushBoolean(ls.Compare(1, 2, CompareOp.Eq));
            // PrintStack(ls);

            // ls.PushNumber(3);
            // ls.PushNumber(3);
            // ls.PushNumber(4);
            // ls.PushBoolean(ls.Compare(-2, -3, CompareOp.Lt));
            // ls.PushBoolean(ls.Compare(-3, -4, CompareOp.Le));
            // ls.PushBoolean(ls.Compare(-4, -3, CompareOp.Lt));
            // PrintStack(ls);

            //// ch6 debug
            //if (args.Length >= 1)
            //{
            //    var data = File.ReadAllBytes(args[0]);
            //    Console.WriteLine("data length: " + data.Length);
            //    Prototype proto = Prototype.Undump(data);
            //    LuaMain(proto);
            //}

            //// ch8 debug
            if (args.Length >= 1)
            {
                var data = File.ReadAllBytes(args[0]);
                Console.WriteLine("data length: " + data.Length);
                var ls = new LuaState();
                ls.Load(data, args[0], "b");
                ls.Call(0, 0);
            }
        }

        //private static void LuaMain(Prototype proto)
        //{
        //    var nRegs = (int)proto.MaxStackSize;
        //    var ls = new LuaState(nRegs + 8, proto);
        //    ls.SetTop(nRegs);
        //    while (true)
        //    {
        //        var pc = ls.PC;
        //        var inst = new Instruction(ls.Fetch());
        //        if (inst.Opcode != (int)Opcodes.OP_RETURN)
        //        {
        //            inst.Execute(ls);
        //            Console.Write($"[{pc + 1:D2}] {inst.OpName}");
        //            PrintStack(ls);
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}
    }
}
