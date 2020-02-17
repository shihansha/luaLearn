using System;
using System.Collections.Generic;

public class LuaUpValue
{
    public LuaValue Value;
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
