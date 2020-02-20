using System;
using System.Collections.Generic;

public partial class LuaState
{
    public bool Compare(int idx1, int idx2, CompareOp op)
    {
        var a = LuaStack.Get(idx1);
        var b = LuaStack.Get(idx2);
        return op switch
        {
            CompareOp.Eq => Eq(a, b, this),
            CompareOp.Lt => Lt(a, b, this),
            CompareOp.Le => Le(a, b, this),
            _ => throw new LuaException("invalid compare op!")
        };

        static bool Lt(LuaValue a, LuaValue b, ILuaState ls)
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
            
            if (LuaValue.TryCallMetamethod(a, b, "__lt", ls, out LuaValue result))
            {
                return (bool)result.Value;
            }

            throw new LuaException("comparison error!");
        }

        static bool Le(LuaValue a, LuaValue b, ILuaState ls)
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

            if (LuaValue.TryCallMetamethod(a, b, "__le", ls, out LuaValue result))
            {
                return (bool)result.Value;
            }
            else if (LuaValue.TryCallMetamethod(b, a, "__lt", ls, out LuaValue result2))
            {
                return !(bool)result2.Value;
            }

            throw new LuaException("comparison error!");
        }

    }

    private static bool Eq(LuaValue a, LuaValue b, ILuaState ls)
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
            case LuaTable av4:
                if (b.Value is LuaTable bv4 && av4 != bv4 && ls != null)
                {
                    if (LuaValue.TryCallMetamethod(av4, bv4, "__eq", ls, out LuaValue result))
                    {
                        return (bool)result.Value;
                    }
                }
                goto default;
            default:
                return a.Value == b.Value;
        }
    }

    public bool RawEqual(int idx1, int idx2)
    {
        if (!LuaStack.IsValid(idx1) || !LuaStack.IsValid(idx2))
        {
            return false;
        }

        var a = LuaStack.Get(idx1);
        var b = LuaStack.Get(idx2);
        return Eq(a, b, null);
    }
}

