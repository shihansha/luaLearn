using System;
using System.Collections.Generic;

public partial class LuaState : ILuaVM
{
    public LuaStack LuaStack { get; private set; }

    public LuaTable Registry { get; internal set; }

    public LuaState()
    {
        Registry = new LuaTable(0, 0);
        Registry.Put(Consts.LUA_RIDX_GLOBALS, new LuaTable(0, 0));

        PushLuaStack(new LuaStack(Consts.LUA_MINSTACK, this));
    }

    public void PushLuaStack(LuaStack stack)
    {
        stack.Prev = this.LuaStack;
        this.LuaStack = stack;
    }

    public void PopLuaStack()
    {
        var stack = this.LuaStack;
        this.LuaStack = stack.Prev;
        stack.Prev = null;
    }

    public static int LuaUpvalueIndex(int i) => Consts.LUA_REGISTRYINDEX - i;
}
