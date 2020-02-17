using System;

public partial class LuaState
{
    public int GetTop()
    {
        return LuaStack.Top;
    }

    public int AbsIndex(int idx)
    {
        return LuaStack.AbsIndex(idx);
    }

    public bool CheckStack(int n)
    {
        LuaStack.Check(n);
        return true;
    }

    public void Pop(int n)
    {
        SetTop(-n - 1);
    }

    public void Copy(int fromIdx, int toIdx)
    {
        var val = LuaStack[fromIdx];
        LuaStack[toIdx] = val;
    }

    public void PushValue(int idx)
    {
        var val = LuaStack[idx];
        LuaStack.Push(val);
    }

    public void Replace(int idx)
    {
        var val = LuaStack.Pop();
        LuaStack.Set(idx, val);
    }

    public void Insert(int idx)
    {
        Rotate(idx, 1);
    }

    public void Remove(int idx)
    {
        Rotate(idx, -1);
        Pop(1);
    }

    public void Rotate(int idx, int n)
    {
        int t = LuaStack.Top - 1;
        int p = LuaStack.AbsIndex(idx) - 1;
        int m = n >= 0 ? t - n : p - n - 1;

        LuaStack.Reverse(p, m);
        LuaStack.Reverse(m + 1, t);
        LuaStack.Reverse(p, t);
    }

    public void SetTop(int idx)
    {
        int newTop = LuaStack.AbsIndex(idx);
        if (newTop < 0)
        {
            throw new Exception("stack underflow!");
        }
        int n = LuaStack.Top - newTop;
        if (n > 0)
        {
            for (int i = 0; i < n; i++)
            {
                LuaStack.Pop();
            }
        }
        else if (n < 0)
        {
            for (int i = 0; i > n; i--)
            {
                LuaStack.Push(new LuaValue(null));
            }
        }
    }
}
