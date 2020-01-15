using System;
using System.Collections.Generic;

public partial class LuaState : ILuaVM
{
    private const int DEFAULT_STACK_SIZE = 20;

    private readonly LuaStack stack;

    private readonly Prototype proto;
    private int pc;

    public LuaState(int stackSize, Prototype proto)
    {
        stack = new LuaStack(stackSize);
        this.proto = proto;
        pc = 0;
    }
}
