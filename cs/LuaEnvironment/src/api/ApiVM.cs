using System;

public partial class LuaState
{
    public int PC => pc;

    public void AddPC(int n)
    {
        pc += n;
    }

    public UInt32 Fetch()
    {
        var i = proto.Code[pc];
        pc++;
        return i;
    }

    public void GetConst(int idx)
    {
        var c = proto.Constants[idx];
        stack.Push(new LuaValue(c));
    }

    public void GetRK(int rk)
    {
        if (rk > 0xff)
        {
            GetConst(rk & 0xff);
        }
        else
        {
            PushValue(rk + 1);
        }
    }
}

