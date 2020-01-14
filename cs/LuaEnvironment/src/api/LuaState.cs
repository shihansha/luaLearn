using System;
using System.Collections.Generic;

public partial class LuaState : ILuaState
{
    private const int DEFAULT_STACK_SIZE = 20;

    private LuaStack stack;
    public static LuaState New()
    {
        return new LuaState { stack = new LuaStack(DEFAULT_STACK_SIZE) };
    }
}
