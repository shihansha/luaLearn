using System;
using System.Collections.Generic;
using System.Text;

public partial class LuaState
{
    public void SetTable(int idx)
    {
        var t = stack.Get(idx);
        var v = stack.Pop();
        var k = stack.Pop();
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
        var t = stack.Get(idx);
        var v = stack.Pop();
        SetTable(t, k, v);
    }

    public void SetI(int idx, Int64 i)
    {
        var t = stack.Get(idx);
        var v = stack.Pop();
        SetTable(t, i, v);
    }
}
