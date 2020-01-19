using System;
using System.Collections.Generic;

public partial class LuaState : ILuaVM
{
    private LuaStack stack;

    public LuaState()
    {
        stack = new LuaStack(20);
    }

    public void PushLuaStack(LuaStack stack)
    {
        stack.Prev = this.stack;
        this.stack = stack;
    }

    public void PopLuaStack()
    {
        var stack = this.stack;
        this.stack = stack.Prev;
        stack.Prev = null;
    }
}
