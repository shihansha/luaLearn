using System;

internal static class LuaDebugUtils
{
    // debug use
    public static void PrintStack(LuaState ls)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        var top = ls.GetTop();
        for (int i = 1; i <= top; i++)
        {
            var t = ls.Type(i);
            switch (t)
            {
                case LuaType.Boolean: builder.Append($"[{ls.ToBoolean(i).ToString().ToLower()}]"); break;
                case LuaType.Number: builder.Append($"[{ls.ToNumber(i)}]"); break;
                case LuaType.String: builder.Append($"[\"{ls.ToString(i)}\"]"); break;
                //case LuaType.Function: builder.Append($"[{(ls.LuaStack.Get(i)?.Value as LuaClosure)?.Proto.LineDefined}]"); break;
                default: builder.Append($"[{ls.TypeName(t)}]"); break;
            }
        }
        Console.WriteLine(builder.ToString());
    }

}


