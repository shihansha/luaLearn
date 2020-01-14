using System;
using System.Collections.Generic;

internal class LuaStack
{
    private List<LuaValue> slots;
    private int top;

    public int Top => top;

    public LuaStack(int size)
    {
        slots = new List<LuaValue>(size);
        for (int i = 0; i < size; i++)
        {
            slots.Add(new LuaValue(null));
        }
        top = 0;
    }

    public void Check(int n)
    {
        int free = slots.Count - top;
        for (int i = free; i < n; i++)
        {
            slots.Add(new LuaValue(null));
        }
    }

    public void Push(LuaValue val)
    {
        if (top == slots.Count)
        {
            throw new Exception("stack overflow!");
        }

        slots[top++] = val;
    }

    public LuaValue Pop()
    {
        if (top < 1)
        {
            throw new Exception("stack underflow!");
        }
        top--;
        LuaValue val = slots[top];
        slots[top] = new LuaValue(null);
        return val;
    }

    public int AbsIndex(int idx)
    {
        if (idx >= 0)
        {
            return idx;
        }

        return idx + top + 1;
    }

    public bool IsValid(int idx)
    {
        int absIdx = AbsIndex(idx);
        return absIdx > 0 && absIdx <= top;
    }

    public void Set(int idx, LuaValue val)
    {
        int absIdx = AbsIndex(idx);
        if (IsValid(absIdx))
        {
            slots[absIdx - 1] = val;
            return;
        }
        throw new Exception("invalid index!");
    }

    public LuaValue Get(int idx)
    {
        int absIdx = AbsIndex(idx);
        if (IsValid(absIdx))
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
