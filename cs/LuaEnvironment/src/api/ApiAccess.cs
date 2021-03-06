using System;

public partial class LuaState
{
    
    public bool IsBoolean(int idx)
    {
        return Type(idx) == LuaType.Boolean;
    }

    public bool IsInteger(int idx)
    {
        var val = LuaStack.Get(idx);
        return val.Value.GetType() == typeof(Int64);
    }

    public bool IsNil(int idx)
    {
        return Type(idx) == LuaType.Nil;
    }

    public bool IsNone(int idx)
    {
        return Type(idx) == LuaType.None;
    }

    public bool IsNoneOrNil(int idx)
    {
        return (int)Type(idx) <= (int)LuaType.Nil;
    }

    public bool IsNumber(int idx)
    {
        var (_, ok) = ToNumberX(idx);
        return ok;
    }

    public bool IsString(int idx)
    {
        var t = Type(idx);
        return t == LuaType.String || t == LuaType.Number;
    }

    public bool IsCSharpFunction(int idx)
    {
        var val = LuaStack.Get(idx);
        if (val.Value is LuaClosure lc)
        {
            return lc.CSharpFunc != null;
        }
        return false;
    }

    public bool ToBoolean(int idx)
    {
        var val = LuaStack.Get(idx);
        return ConvertToBoolean(val);

        static bool ConvertToBoolean(LuaValue val) => val.Value switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }

    public long ToInteger(int idx)
    {
        var (i, _) = ToIntegerX(idx);
        return i;
    }

    public (long, bool) ToIntegerX(int idx)
    {
        var val = LuaStack.Get(idx);
        return val.ToInteger();
    }

    public double ToNumber(int idx)
    {
        var (n, _) = ToNumberX(idx);
        return n;
    }

    public (double, bool) ToNumberX(int idx)
    {
        var val = LuaStack.Get(idx);
        return val.ToFloat();
    }

    public string ToString(int idx)
    {
        var (s, _) = ToStringX(idx);
        return s;
    }

    public (string, bool) ToStringX(int idx)
    {
        var val = LuaStack.Get(idx);
        switch (val.Value)
        {
            case string s: return (s, true);
            case object o when o is Int64 || o is double:
                string os = o.ToString();
                LuaStack.Set(idx, new LuaValue(os));
                return (os, true);
            default:
                return (string.Empty, false);
        }
    }

    public LuaType Type(int idx)
    {
        if (LuaStack.IsValid(idx))
        {
            var val = LuaStack[idx];
            return LuaValue.TypeOf(val);
        }
        return LuaType.None;
    }

    public CSharpFunction ToCSharpFunction(int idx)
    {
        var val = LuaStack.Get(idx);
        if (val.Value is LuaClosure lc)
        {
            return lc.CSharpFunc;
        }
        return null;
    }

    public string TypeName(LuaType tp) => tp switch
    {
        LuaType.None => "no value",
        LuaType.Nil => "nil",
        LuaType.Boolean => "boolean",
        LuaType.Number => "number",
        LuaType.String => "string",
        LuaType.Table => "table",
        LuaType.Thread => "thread",
        _ => "userdata"
    };
}
