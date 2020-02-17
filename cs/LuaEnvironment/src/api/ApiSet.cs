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
        SetTable(t, k, v);
    }

    private void SetTable(LuaValue t, LuaValue k, LuaValue v)
    {
        if (t.Value is LuaTable tbl)
        {
            tbl.Put(k, v);
            return;
        }

        throw new Exception("not a table!");
    }

    public void SetField(int idx, string k)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        SetTable(t, k, v);
    }

    public void SetI(int idx, Int64 i)
    {
        var t = LuaStack.Get(idx);
        var v = LuaStack.Pop();
        SetTable(t, i, v);
    }

    public void SetGlobal(string name)
    {
        var t = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        var v = LuaStack.Pop();
        SetTable(t, name, v);
    }

    public void Register(string name, CSharpFunction cs)
    {
        PushCSharpFunction(cs);
        SetGlobal(name);
    }
}
