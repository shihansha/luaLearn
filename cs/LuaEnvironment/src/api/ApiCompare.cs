using System;
using System.Collections.Generic;

public partial class LuaState
{
    public bool Compare(int idx1, int idx2, CompareOp op)
    {
        var a = stack.Get(idx1);
        var b = stack.Get(idx2);
        return op switch
        {
            CompareOp.Eq => Eq(a, b),
            CompareOp.Lt => Lt(a, b),
            CompareOp.Le => Le(a, b),
            _ => throw new Exception("invalid compare op!")
        };

        bool Eq(LuaValue a, LuaValue b)
        {
            switch (a.Value)
            {
                case null: return b.Value == null;
                case bool av:
                    if (b.Value is bool bv)
                    {
                        return av == bv;
                    }
                    return false;
                case string av1:
                    if (b.Value is string bv1)
                    {
                        return av1 == bv1;
                    }
                    return false;
                case Int64 av2:
                    switch (b.Value)
                    {
                        case Int64 i: return i == av2;
                        case double d: return (double)av2 == d;
                        default: return false;
                    }
                case double av3:
                    switch (b.Value)
                    {
                        case double d: return av3 == d;
                        case Int64 i: return av3 == (double)i;
                        default: return false;
                    }
                default:
                    return a.Value == b.Value;
            }
        }

        bool Lt(LuaValue a, LuaValue b)
        {
            switch (a.Value)
            {
                case string av:
                    if (b.Value is string bv)
                    {
                        return string.Compare(av, bv) < 0;
                    }
                    break;
                case Int64 av1:
                    switch (b.Value)
                    {
                        case Int64 i: return av1 < i;
                        case double d: return (double)av1 < d;
                    }
                    break;
                case double av2:
                    switch (b.Value)
                    {
                        case double d: return av2 < d;
                        case Int64 i: return av2 < (double)i;
                    }
                    break;
                default: break;
            }
            throw new Exception("comparison error!");
        }

        bool Le(LuaValue a, LuaValue b)
        {
            switch (a.Value)
            {
                case string av:
                    if (b.Value is string bv)
                    {
                        return string.Compare(av, bv) <= 0;
                    }
                    break;
                case Int64 av1:
                    switch (b.Value)
                    {
                        case Int64 i: return av1 <= i;
                        case double d: return (double)av1 <= d;
                    }
                    break;
                case double av2:
                    switch (b.Value)
                    {
                        case double d: return av2 <= d;
                        case Int64 i: return av2 <= (double)i;
                    }
                    break;
                default: break;
            }
            throw new Exception("comparison error!");
        }

    }
}

