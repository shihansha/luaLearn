using System;
using System.Collections.Generic;

public class LuaStack
{
    private readonly LuaState state;
    private readonly List<LuaValue> slots;

    public int Top { get; set; }

    public LuaStack Prev;
    public LuaClosure Closure;
    public List<LuaValue> Varargs;
    public int PC;
    public Dictionary<int, LuaUpValue> Openuvs;

    public List<LuaValue> Slots { get => slots; }

    public LuaStack(int size, LuaState state)
    {
        slots = new List<LuaValue>(size);
        for (int i = 0; i < size; i++)
        {
            slots.Add(new LuaValue(null));
        }
        Top = 0;
        this.state = state;
    }

    public void Check(int n)
    {
        int free = slots.Count - Top;
        for (int i = free; i < n; i++)
        {
            slots.Add(new LuaValue(null));
        }
    }

    public void Push(LuaValue val)
    {
        if (Top == slots.Count)
        {
            throw new Exception("stack overflow!");
        }

        slots[Top++] = val;
    }

    public LuaValue Pop()
    {
        if (Top < 1)
        {
            throw new Exception("stack underflow!");
        }
        Top--;
        LuaValue val = slots[Top];
        slots[Top] = new LuaValue(null);
        return val;
    }

    public int AbsIndex(int idx)
    {
        if (idx <= Consts.LUA_REGISTRYINDEX)
        {
            return idx;
        }

        if (idx >= 0)
        {
            return idx;
        }

        return idx + Top + 1;
    }

    public bool IsValid(int idx)
    {
        if (idx < Consts.LUA_REGISTRYINDEX)
        {
            int uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
            var c = Closure;
            return c != null && uvIdx < (c.Upvals?.Length ?? 0);
        }
        if (idx == Consts.LUA_REGISTRYINDEX)
        {
            return true;
        }

        int absIdx = AbsIndex(idx);
        return absIdx > 0 && absIdx <= Top;
    }

    public void Set(int idx, LuaValue val)
    {
        if (idx < Consts.LUA_REGISTRYINDEX)
        {
            int uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
            var c = Closure;
            if (c != null && uvIdx < (c.Upvals?.Length ?? 0))
            {
                c.Upvals[uvIdx] = LuaUpValue.CreateClosed(val);
            }
            return;
        }

        if (idx == Consts.LUA_REGISTRYINDEX)
        {
            state.Registry = val.Value as LuaTable;
            return;
        }

        int absIdx = AbsIndex(idx);
        if (absIdx > 0 && absIdx <= Top)
        {
            slots[absIdx - 1] = val;
            return;
        }
        throw new Exception("invalid index!");
    }

    internal void PushN(LuaValue[] vals, int n)
    {
        var nVals = vals.Length;
        if (n < 0)
        {
            n = nVals;
        }
        for (int i = 0; i < n; i++)
        {
            if (i < nVals)
            {
                Push(vals[i]);
            }
            else
            {
                Push(new LuaValue(null));
            }
        }
    }

    internal LuaValue[] PopN(int n)
    {
        var vals = new LuaValue[n];
        for (int i = n - 1; i >= 0; i--)
        {
            vals[i] = Pop();
        }
        return vals;
    }

    public LuaValue Get(int idx)
    {
        if (idx < Consts.LUA_REGISTRYINDEX)
        {
            int uvIdx = Consts.LUA_REGISTRYINDEX - idx - 1;
            var c = Closure;
            if (c == null || uvIdx >= (c.Upvals?.Length ?? 0))
            {
                return new LuaValue(null);
            }
            return c.Upvals[uvIdx].Value;
        }

        if (idx == Consts.LUA_REGISTRYINDEX)
        {
            return state.Registry;
        }

        int absIdx = AbsIndex(idx);
        if (absIdx > 0 && absIdx <= Top)
        {
            return slots[absIdx - 1];
        }
        return new LuaValue(null);
    }

    public LuaValue this[int idx]
    {
        get => Get(idx);
        set => Set(idx, value);
    }

    public void Reverse(int from, int to)
    {
        var slots = this.slots;
        while (from < to)
        {
            (slots[from], slots[to]) = (slots[to], slots[from]);
            from++;
            to--;
        }
    }
}
