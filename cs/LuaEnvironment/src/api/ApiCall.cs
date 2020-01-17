using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class LuaState
{
    public int Load(byte[] chunk, string chunkName, string mode)
    {
        var proto = Prototype.Undump(chunk);
        var c = new LuaClosure(proto);
        stack.Push(c);
        return 0;
    }

    public void Call(int nArgs, int nResults)
    {
        var val = stack.Get(-(nArgs + 1));
        if (val.Value is LuaClosure c)
        {
            Console.WriteLine($"call {c.Proto.Source}<{c.Proto.LineDefined},{c.Proto.LastLineDefined}>");
            CallLuaClosure(nArgs, nResults, c);
        }
        else
        {
            throw new Exception("not function!");
        }
    }

    private void CallLuaClosure(int nArgs, int nResults, LuaClosure c)
    {
        int nRegs = c.Proto.MaxStackSize;
        int nParams = c.Proto.NumParams;
        bool isVararg = c.Proto.IsVararg == 1;

        var newStack = new LuaStack(nRegs + 20)
        {
            Closure = c
        };

        LuaValue[] funcAndArgs = stack.PopN(nArgs + 1);
        newStack.PushN(funcAndArgs[1..], nParams);
        newStack.Top = nArgs;
        if (nArgs > nParams && isVararg)
        {
            newStack.Varargs = funcAndArgs[nParams..].ToList();
        }

        PushLuaStack(newStack);
        RunLuaClosure();
        PopLuaStack();

        if (nResults != 0)
        {
            LuaValue[] results = newStack.PopN(newStack.Top - nRegs);
            stack.Check(results.Length);
            stack.PushN(results, nResults);
        }
    }

    private void RunLuaClosure()
    {
        while (true)
        {
            Instruction inst = Fetch();
            inst.Execute(this);
            if ((Opcodes)inst.Opcode == Opcodes.OP_RETURN)
            {
                break;
            }
        }
    }
}

