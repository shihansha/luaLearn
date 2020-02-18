using System;

public partial class LuaState
{
    public void PushNil() => LuaStack.Push(new LuaValue(null));
    public void PushBoolean(bool b) => LuaStack.Push(b);
    public void PushInteger(Int64 n) => LuaStack.Push(n);
    public void PushNumber(double n) => LuaStack.Push(n);
    public void PushString(string s) => LuaStack.Push(s);
    public void PushCSharpFunction(CSharpFunction cs) => LuaStack.Push(new LuaClosure(cs, 0));
    public void PushGlobalTable()
    {
        var gl = Registry.Get(Consts.LUA_RIDX_GLOBALS);
        LuaStack.Push(gl);
    }
    public void PushCSharpClosure(CSharpFunction f, int n)
    {
        var closure = new LuaClosure(f, n);
        for (int i = n - 1; i >= 0; i--)
        {
            var val = LuaStack.Pop();
            closure.Upvals[i] = LuaUpValue.CreateClosed(val); 
        }
        LuaStack.Push(closure);
    }
}
