using System;

public partial class LuaState : ILuaState
{
    public void PushNil() => stack.Push(new LuaValue(null));
    public void PushBoolean(bool b) => stack.Push(new LuaValue(b));
    public void PushIneger(Int64 n) => stack.Push(new LuaValue(n));
    public void PushNumber(double n) => stack.Push(new LuaValue(n));
    public void PushString(string s) => stack.Push(new LuaValue(s));
    public void PushInteger(long n) => stack.Push(new LuaValue(n));
}
