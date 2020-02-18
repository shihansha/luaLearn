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
        LuaStack.Push(c);
        if (proto.UpValues != null && proto.UpValues.Length > 0)
        {
            var env = Registry.Get(Consts.LUA_RIDX_GLOBALS);
            c.Upvals[0] = LuaUpValue.CreateClosed(env);
        }
        return 0;
    }

    public void Call(int nArgs, int nResults)
    {
        var val = LuaStack.Get(-(nArgs + 1));
        var f = val.Value as LuaClosure;
        if (f == null)
        {
            if (LuaValue.GetMetaField(val, "__call", this) is LuaValue mf)
            {
                if (mf.Value is LuaClosure c)
                {
                    LuaStack.Push(val);
                    Insert(-(nArgs + 2));
                    nArgs += 1;
                    f = c;
                }
            }
        }
        if (f != null)
        {
            if (f.Proto != null)
            {
                CallLuaClosure(nArgs, nResults, f);
            }
            else
            {
                CallCSharpClosure(nArgs, nResults, f);
            }
        }
        else
        {
            throw new Exception("not function!");
        }
    }

    private void CallCSharpClosure(int nArgs, int nResults, LuaClosure c)
    {
        var newStack = new LuaStack(nArgs + 20, this)
        {
            Closure = c
        };

        var args = LuaStack.PopN(nArgs);
        newStack.PushN(args, nArgs);
        LuaStack.Pop();

        PushLuaStack(newStack);
        int r = c.CSharpFunc.Invoke(this);
        PopLuaStack();

        if (nResults != 0)
        {
            var results = newStack.PopN(r);
            LuaStack.Check(nResults);
            LuaStack.PushN(results, nResults);
        }
    }

    private void CallLuaClosure(int nArgs, int nResults, LuaClosure c)
    {
        int nRegs = c.Proto.MaxStackSize;
        int nParams = c.Proto.NumParams;
        bool isVararg = c.Proto.IsVararg == 1;

        var newStack = new LuaStack(nRegs + 20, this)
        {
            Closure = c
        };

        LuaValue[] funcAndArgs = LuaStack.PopN(nArgs + 1);
        newStack.PushN(funcAndArgs[1..], nParams);
        newStack.Top = nRegs;
        if (nArgs > nParams && isVararg)
        {
            newStack.Varargs = funcAndArgs[(nParams + 1)..].ToList();
        }

        PushLuaStack(newStack);
        RunLuaClosure();
        PopLuaStack();

        if (nResults != 0)
        {
            LuaValue[] results = newStack.PopN(newStack.Top - nRegs);
            LuaStack.Check(nResults);
            LuaStack.PushN(results, nResults);
        }
    }

    private void RunLuaClosure()
    {
        while (true)
        {
            Instruction inst = Fetch();
            //LuaDebugUtils.PrintStack(this);
            //Console.WriteLine(OpCodeInfo.OpCodeInfos[inst.Opcode].Name);
            inst.Execute(this);
            if ((Opcodes)inst.Opcode == Opcodes.OP_RETURN)
            {
                break;
            }
        }
    }
}

