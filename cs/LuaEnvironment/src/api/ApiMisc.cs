using System;
using System.Collections.Generic;

public partial class LuaState
{
    public void Len(int idx)
    {
        var val = stack.Get(idx);
        if (val.Value is string s)
        {
            stack.Push(new LuaValue((Int64)s.Length));
        }
        else if (val.Value is LuaTable t)
        {
            stack.Push(t.Len);
        }
        else
        {
            throw new Exception("lengh error!");
        }
    }

    public void Concat(int n)
    {
        if (n == 0)
        {
            stack.Push(new LuaValue(string.Empty));
        }
        else if (n >= 2)
        {
            for (int i = 1; i < n; i++)
            {
                if (IsString(-1) && IsString(-2))
                {
                    string s2 = ToString(-1);
                    string s1 = ToString(-2);
                    stack.Pop();
                    stack.Pop();
                    stack.Push(new LuaValue(s1 + s2));
                    continue;
                }
                throw new Exception("concatenation error!");
            }
        }
        // n == 1, do nothing
    }
}
