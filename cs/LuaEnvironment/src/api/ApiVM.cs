using System;

public partial class LuaState
{
    public int PC => stack.PC;

    public void AddPC(int n)
    {
        stack.PC += n;
    }

    public UInt32 Fetch()
    {
        var i = stack.Closure.Proto.Code[stack.PC];
        stack.PC++;
        return i;
    }

    public void GetConst(int idx)
    {
        var c = stack.Closure.Proto.Constants[idx];
        stack.Push(new LuaValue(c));
    }

    public void GetRK(int rk)
    {
        if (rk > 0xff)
        {
            GetConst(rk & 0xff);
        }
        else
        {
            PushValue(rk + 1);
        }
    }

    public int RegisterCount()
    {
        return stack.Closure.Proto.MaxStackSize;
    }

    public void LoadVararg(int n)
    {
        if (n < 0)
        {
            n = stack.Varargs.Count;
        }
        stack.Check(n);
        stack.PushN(stack.Varargs.ToArray(), n);
    }

    public void LoadProto(int idx)
    {
        var proto = stack.Closure.Proto.Protos[idx];
        var closure = new LuaClosure(proto);
        stack.Push(closure);
    }
}

