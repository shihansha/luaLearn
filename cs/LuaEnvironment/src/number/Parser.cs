using System;

internal static partial class Number
{
    public static (Int64, bool) ParseInteger(string str)
    {
        bool success = Int64.TryParse(str, out Int64 result);
        return (result, success);
    }

    public static (double, bool) ParseFloat(string str)
    {
        bool success = double.TryParse(str, out double result);
        return (result, success);
    }
}

