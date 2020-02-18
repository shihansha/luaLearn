using System;
using System.Collections.Generic;

public class LuaUpValue
{
    public bool IsOpen { get; private set; }
    private int idx;
    private LuaValue value;
    public LuaValue Value
    {
        get
        {
            if (IsOpen)
            {
                return stack.Slots[idx];
            }
            else
            {
                return value;
            }
        }
    }
    private LuaStack stack;
    private LuaUpValue() { }
    public static LuaUpValue CreateOpen(LuaStack stack, int idx)
    {
        var result = new LuaUpValue()
        {
            IsOpen = true,
            idx = idx,
            stack = stack
        };
        return result;
    }

    public static LuaUpValue CreateClosed(LuaValue value)
    {
        var result = new LuaUpValue()
        {
            IsOpen = false,
            value = value
        };
        return result;
    }

    public void MakeClosed()
    {
        IsOpen = false;
        value = stack.Slots[idx];
    }
}

public class LuaClosure
{
    public Prototype Proto = null;
    public CSharpFunction CSharpFunc = null;
    public LuaUpValue[] Upvals;

    public LuaClosure(Prototype proto)
    {
        Proto = proto;
        int nUpvals = proto.UpValues.Length;
        if (nUpvals > 0)
        {
            Upvals = new LuaUpValue[nUpvals];
        }
    }

    public LuaClosure(CSharpFunction cSharpFunction, int nUpvals)
    {
        CSharpFunc = cSharpFunction;
        if (nUpvals > 0)
        {
            Upvals = new LuaUpValue[nUpvals];
        }
    }
}
