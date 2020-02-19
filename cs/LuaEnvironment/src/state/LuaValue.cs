using System;
using System.Collections.Generic;

// LuaValue should never be null
public class LuaValue
{
    public object Value;

    public static implicit operator LuaValue(Int64 obj) => new LuaValue(obj);
    public static implicit operator LuaValue(double obj) => new LuaValue(obj);
    public static implicit operator LuaValue(string obj) => new LuaValue(obj);
    public static implicit operator LuaValue(bool obj) => new LuaValue(obj);
    public static implicit operator LuaValue(LuaTable obj) => new LuaValue(obj);
    public static implicit operator LuaValue(LuaClosure obj) => new LuaValue(obj);

    public LuaValue(object obj)
    {
        Value = obj;
    }

    public static LuaValue Nil { get; } = new LuaValue(null);

    public static LuaType TypeOf(LuaValue val) => val.Value switch
    {
        null => LuaType.Nil,
        bool _ => LuaType.Boolean,
        Int64 _ => LuaType.Number,
        double _ => LuaType.Number,
        string _ => LuaType.String,
        LuaTable _ => LuaType.Table,
        LuaClosure _ => LuaType.Function,
        _ => throw new Exception($"Type {val.Value.GetType()} is not a lua type!")
    };

    public (double, bool) ToFloat() => Value switch
    {
        double n => (n, true),
        Int64 n => ((double)n, true),
        string s => Number.ParseFloat(s),
        _ => (0, false)
    };

    public (Int64, bool) ToInteger() 
    {
        return Value switch
        {
            Int64 n => (n, true),
            double n => Number.FloatToInteger(n),
            string s => StringToInteger(s),
            _ => (0, false)
        };

        static (Int64, bool) StringToInteger(string str)
        {
            var (i, ok) = Number.ParseInteger(str);
            if (ok) return (i, true);
            var (n, ok2) = Number.ParseFloat(str);
            if (ok2) return Number.FloatToInteger(n);
            return (0, false);
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public override bool Equals(object obj)
    {
        if (obj is LuaValue val)
        {
            return Equals(Value, val.Value);
        }
        return false;
    }

    public static bool operator ==(LuaValue left, LuaValue right)
    {
        return Equals(left?.Value, right?.Value);
    }

    public static bool operator !=(LuaValue left, LuaValue right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static void SetMetatable(LuaValue val, LuaTable mt, LuaState ls)
    {
        if (val.Value is LuaTable t)
        {
            t.Metatable = mt;
            return;
        }
        string key = $"_MT{val.GetType()}";
        ls.Registry.Put(key, mt);
    }

    public static LuaTable GetMetatable(LuaValue val, ILuaState ls)
    {
        if (val.Value is LuaTable t)
        {
            return t.Metatable;
        }
        string key = $"_MT{val.GetType()}";
        return ls.Registry.Get(key)?.Value as LuaTable;
    }

    public static bool TryCallMetamethod(LuaValue a, LuaValue b, string mmName, ILuaState ls, out LuaValue result)
    {
        LuaValue mm;
        mm = GetMetaField(a, mmName, ls);
        if (mm == null)
        {
            mm = GetMetaField(b, mmName, ls);
            if (mm == null)
            {
                result = LuaValue.Nil;
                return false;
            }
        }

        ls.LuaStack.Check(4);
        ls.LuaStack.Push(mm);
        ls.LuaStack.Push(a);
        ls.LuaStack.Push(b);
        ls.Call(2, 1);
        result = ls.LuaStack.Pop();
        return true;
    }

    public static LuaValue GetMetaField(LuaValue val, string fieldName, ILuaState ls)
    {
        return GetMetatable(val, ls)?.Get(fieldName);
    }
}
