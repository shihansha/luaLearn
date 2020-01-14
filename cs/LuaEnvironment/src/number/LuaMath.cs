using System;

internal static partial class Number
{
    public static Int64 FloorDiv(Int64 a, Int64 b)
    {
        if ((a > 0 && b > 0) || (a < 0 && b < 0) || a % b == 0)
        {
            return a / b;
        }
        else
        {
            return a / b - 1;
        }
    }

    public static double FloorDiv(double a, double b)
    {
        return Math.Floor(a / b);
    }

    public static Int64 Mod(Int64 a, Int64 b)
    {
        return a - FloorDiv(a, b) * b;
    }

    public static double Mod(double a, double b)
    {
        return a - FloorDiv(a, b) * b;
    }

    public static Int64 ShiftLeft(Int64 a, Int64 n)
    {
        if (n >= 0)
        {
            return a << (int)n;
        }
        else
        {
            return ShiftRight(a, -n);
        }
    }

    public static Int64 ShiftRight(Int64 a, Int64 n)
    {
        if (n >= 0)
        {
            return (Int64)((UInt64)a >> (int)n);
        }
        else
        {
            return ShiftLeft(a, -n);
        }
    }

    public static (Int64, bool) FloatToInteger(double f)
    {
        Int64 i = (Int64)f;
        return (i, (double)i == f);
    }
}