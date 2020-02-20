using System;
using System.Collections.Generic;
using System.Text;

public partial class LuaState
{
    public void SetTable(int idx)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        var k = LuaStack.Pop();
        SetTable(t, k, v, false);
    }

    public void RawSet(int idx)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        var k = LuaStack.Pop();
        SetTable(t, k, v, true);
    }

    private void SetTable(LuaValue t, LuaValue k, LuaValue v, bool raw)
    {
        if (t.Value is LuaTable tbl)
        {
            if (raw || tbl.Get(k).Value != null || !tbl.HasMetafield("__newindex"))
            {
                tbl.Put(k, v);
                return;
            }
        }

        if (!raw)
        {
            if (LuaValue.GetMetaField(t, "__newindex", this) is LuaValue mf)
            {
                switch (mf.Value)
                {
                    case LuaTable x:
                        SetTable(x, k, v, false);
                        return;
                    case LuaClosure _:
                        {
                            LuaStack.Push(mf);
                            LuaStack.Push(t);
                            LuaStack.Push(k);
                            LuaStack.Push(v);
                            Call(3, 0);
                            return;
                        }
                }
            }
        }

        throw new LuaException("not a table!");
    }

    public void SetField(int idx, string k)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        SetTable(t, k, v, false);
    }

    public void SetI(int idx, Int64 i)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        SetTable(t, i, v, false);
    }

    public void RawSetI(int idx, Int64 i)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        SetTable(t, i, v, true);
    }

    public void SetGlobal(string name)
    {
        var t = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        var v = LuaStack.Pop();
        SetTable(t, name, v, false);
    }

    public void RawSetGlobal(string name)
    {
        var t = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        var v = LuaStack.Pop();
        SetTable(t, name, v, true);
    }

    public void Register(string name, CSharpFunction cs)
    {
        PushCSharpFunction(cs);
        SetGlobal(name);
    }

    public void SetMetatable(int idx)
    {
        var val = LuaStack.Get(idx);
        var mtVal = LuaStack.Pop();

        if (mtVal == null)
        {
            LuaValue.SetMetatable(val, null, this);
        }
        else if (mtVal.Value is LuaTable mt)
        {
            LuaValue.SetMetatable(val, mt, this);
        }
        else
        {
            throw new LuaException("table expected!");
        }
    }
}
