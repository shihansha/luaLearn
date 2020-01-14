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
}
