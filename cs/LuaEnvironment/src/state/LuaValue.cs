using System;
using System.Collections.Generic;

// LuaValue should never be null
internal struct LuaValue
{
    public object Value;

    public LuaValue(object obj)
    {
        Value = obj;
    }

    public static LuaType TypeOf(LuaValue val) => val.Value switch
    {
        null => LuaType.Nil,
        bool _ => LuaType.Boolean,
        Int64 _ => LuaType.Number,
        double _ => LuaType.Number,
        string _ => LuaType.String,
        _ => throw new Exception($"Type {val.GetType()} is not a lua type!")
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

        (Int64, bool) StringToInteger(string str)
        {
            var (i, ok) = Number.ParseInteger(str);
            if (ok) return (i, true);
            var (n, ok2) = Number.ParseFloat(str);
            if (ok2) return Number.FloatToInteger(n);
            return (0, false);
        }
    }
}
