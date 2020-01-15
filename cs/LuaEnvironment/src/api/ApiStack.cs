using System;

public partial class LuaState
{
    public int GetTop()
    {
        return stack.Top;
    }

    public int AbsIndex(int idx)
    {
        return stack.AbsIndex(idx);
    }

    public bool CheckStack(int n)
    {
        stack.Check(n);
        return true;
    }

    public void Pop(int n)
    {
        SetTop(-n - 1);
    }

    public void Copy(int fromIdx, int toIdx)
    {
        var val = stack[fromIdx];
        stack[toIdx] = val;
    }

    public void PushValue(int idx)
    {
        var val = stack[idx];
        stack.Push(val);
    }

    public void Replace(int idx)
    {
        var val = stack.Pop();
        stack[idx] = val;
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
        int t = stack.Top - 1;
        int p = stack.AbsIndex(idx) - 1;
        int m = n >= 0 ? t - n : p - n - 1;

        stack.Reverse(p, m);
        stack.Reverse(m + 1, t);
        stack.Reverse(p, t);
    }

    public void SetTop(int idx)
    {
        int newTop = stack.AbsIndex(idx);
        if (newTop < 0)
        {
            throw new Exception("stack underflow!");
        }
        int n = stack.Top - newTop;
        if (n > 0)
        {
            for (int i = 0; i < n; i++)
            {
                stack.Pop();
            }
        }
        else if (n < 0)
        {
            for (int i = 0; i > n; i--)
            {
                stack.Push(new LuaValue(null));
            }
        }
    }
}
