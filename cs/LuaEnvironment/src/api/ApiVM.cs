using System;
using System.Collections.Generic;

public partial class LuaState
{
    public int PC => LuaStack.PC;

    public void AddPC(int n)
    {
        LuaStack.PC += n;
    }

    public UInt32 Fetch()
    {
        var i = LuaStack.Closure.Proto.Code[LuaStack.PC];
        LuaStack.PC++;
        return i;
    }

    public void GetConst(int idx)
    {
        var c = LuaStack.Closure.Proto.Constants[idx];
        LuaStack.Push(new LuaValue(c));
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
        return LuaStack.Closure.Proto.MaxStackSize;
    }

    public void LoadVararg(int n)
    {
        if (n < 0)
        {
            n = LuaStack.Varargs.Count;
        }
        LuaStack.Check(n);
        LuaStack.PushN(LuaStack.Varargs.ToArray(), n);
    }

    public void LoadProto(int idx)
    {
        var subProto = LuaStack.Closure.Proto.Protos[idx];
        var closure = new LuaClosure(subProto);
        LuaStack.Push(closure);
        for (int i = 0; i < subProto.UpValues.Length; i++)
        {
            var uvInfo = subProto.UpValues[i];
            int uvIdx = uvInfo.Idx;
            if (uvInfo.InStack == 1)
            {
                if (LuaStack.Openuvs == null)
                {
                    LuaStack.Openuvs = new Dictionary<int, LuaUpValue>();
                }
                if (LuaStack.Openuvs.TryGetValue(uvIdx, out LuaUpValue openuv))
                {
                    closure.Upvals[i] = openuv;
                }
                else
                {
                    closure.Upvals[i] = LuaUpValue.CreateOpen(LuaStack, uvIdx);
                    LuaStack.Openuvs[uvIdx] = closure.Upvals[i];
                }
            }
            else
            {
                closure.Upvals[i] = LuaStack.Closure.Upvals[uvIdx];
            }
        }
    }

    public void CloseUpvalues(int a)
    {
        foreach (LuaUpValue val in LuaStack.Openuvs.Values)
        {
            if (val.IsOpen)
            {
                val.MakeClosed();
            }
        }
    }
}

