using System;
using System.Collections.Generic;
using System.Text;

public partial class LuaState
{
    public void CreateTable(int nArr, int nRec)
    {
        var t = new LuaTable(nArr, nRec);
        LuaStack.Push(t);
    }

    public void NewTable() => CreateTable(0, 0);

    public LuaType GetTable(int idx)
    {
        var t = LuaStack.Get(idx);
        var k = LuaStack.Pop();
        return GetTable(t, k, false);
    }

    public LuaType RawGet(int idx)
    {
        var t = LuaStack.Get(idx);
        var k = LuaStack.Pop();
        return GetTable(t, k, true);
    }


    private LuaType GetTable(LuaValue t, LuaValue k, bool raw)
    {
        if (t.Value is LuaTable tbl)
        {
            var v = tbl.Get(k);
            if (raw || v.Value != null || !tbl.HasMetafield("__index"))
            {
                LuaStack.Push(v);
                return LuaValue.TypeOf(v);
            }
        }

        if (!raw)
        {
            if (LuaValue.GetMetaField(t, "__index", this) is LuaValue mf)
            {
                switch (mf.Value)
                {
                    case LuaTable x:
                        return GetTable(x, k, false);
                    case LuaClosure _:
                        {
                            LuaStack.Push(mf);
                            LuaStack.Push(t);
                            LuaStack.Push(k);
                            Call(2, 1);
                            var v = LuaStack.Get(-1);
                            return LuaValue.TypeOf(v);
                        }
                }
            }
        }

        throw new LuaException("not a table!");
    }

    public LuaType GetField(int idx, string k)
    {
        var t = LuaStack.Get(idx);
        return GetTable(t, k, false);
    }

    public LuaType GetI(int idx, Int64 i)
    {
        var t = LuaStack.Get(idx);
        return GetTable(t, i, false);
    }

    public LuaType RawGetI(int idx, Int64 i)
    {
        var t = LuaStack.Get(idx);
        return GetTable(t, i, true);
    }


    public LuaType GetGlobal(string name)
    {
        var t = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        return GetTable(t, name, false);
    }

    public LuaType RawGetGlobal(string name)
    {
        var t = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        return GetTable(t, name, true);
    }

    public bool GetMetatable(int idx)
    {
        var val = LuaStack.Get(idx);

        if (LuaValue.GetMetatable(val, this) is LuaTable mt)
        {
            LuaStack.Push(mt);
            return true;
        }
        else
        {
            return false;
        }
    }
}

