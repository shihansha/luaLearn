using System;
using System.Collections.Generic;
using System.Text;

public partial class LuaState
{
    public void CreateTable(int nArr, int nRec)
    {
        var t = new LuaTable(nArr, nRec);
        stack.Push(t);
    }

    public void NewTable() => CreateTable(0, 0);

    public LuaType GetTable(int idx)
    {
        var t = stack.Get(idx);
        var k = stack.Pop();
        return GetTable(t, k);
    }

    private LuaType GetTable(LuaValue t, LuaValue k)
    {
        if (t.Value is LuaTable tbl)
        {
            var v = tbl.Get(k);
            stack.Push(v);
            return LuaValue.TypeOf(v);
        }

        throw new Exception("not a table!");
    }

    public LuaType GetField(int idx, string k)
    {
        var t = stack.Get(idx);
        return GetTable(t, k);
    }

    public LuaType GetI(int idx, Int64 i)
    {
        var t = stack.Get(idx);
        return GetTable(t, i);
    }
}

