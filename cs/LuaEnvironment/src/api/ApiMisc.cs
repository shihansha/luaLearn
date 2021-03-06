using System;
using System.Collections.Generic;

public partial class LuaState
{
    public void Len(int idx)
    {
        var val = LuaStack.Get(idx);
        if (val.Value is string s)
        {
            LuaStack.Push(new LuaValue((Int64)s.Length));
        }
        else if (LuaValue.TryCallMetamethod(val, val, "__len", this, out LuaValue result))
        {
            LuaStack.Push(result);
        }
        else if (val.Value is LuaTable t)
        {
            LuaStack.Push(t.Len);
        }
        else
        {
            throw new LuaException("lengh error!");
        }
    }

    public uint RawLen(int idx)
    {
        var val = LuaStack.Get(idx);
        if (val.Value is string s)
        {
            return (uint)s.Length;
        }
        else if (val.Value is LuaTable t)
        {
            return (uint)t.Len;
        }

        return 0;
    }

    public void Concat(int n)
    {
        if (n == 0)
        {
            LuaStack.Push(new LuaValue(string.Empty));
        }
        else if (n >= 2)
        {
            for (int i = 1; i < n; i++)
            {
                if (IsString(-1) && IsString(-2))
                {
                    string s2 = ToString(-1);
                    string s1 = ToString(-2);
                    LuaStack.Pop();
                    LuaStack.Pop();
                    LuaStack.Push(new LuaValue(s1 + s2));
                    continue;
                }
                
                var b = LuaStack.Pop();
                var a = LuaStack.Pop();
                if (LuaValue.TryCallMetamethod(a, b, "__concat", this, out LuaValue result))
                {
                    LuaStack.Push(result);
                    continue;
                }

                throw new LuaException("concatenation error!");
            }
        }
        // n == 1, do nothing
    }

    public bool Next(int idx)
    {
        var val = LuaStack.Get(idx);
        if (val.Value is LuaTable t)
        {
            var key = LuaStack.Pop();
            var nextKey = t.NextKey(key);
            if (nextKey.Value != null)
            {
                LuaStack.Push(nextKey);
                LuaStack.Push(t.Get(nextKey));
                return true;
            }
            return false;
        }
        throw new LuaException("table expected!");
    }

    public int Error()
    {
        var err = LuaStack.Pop();
        throw new LuaException(err);
    }

    public ErrState PCall(int nArgs, int nResults, int msgh)
    {
        var caller = LuaStack;
        var status = ErrState.ErrRun;

        try
        {
            Call(nArgs, nResults);
            status = ErrState.Ok;
            return status;
        }
        catch (LuaException e)
        {
            while (LuaStack != caller)
            {
                PopLuaStack();
            }

            LuaStack.Push(e.Content);
        }

        return status;
    }
}
