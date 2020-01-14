using System;
using System.Collections.Generic;

internal class Operator
{
    public readonly Func<Int64, Int64, Int64> IntegerFunc;
    public readonly Func<double, double, double> FloatFunc;
    public Operator(Func<Int64, Int64, Int64> i, Func<double, double, double> d)
    {
        IntegerFunc = i;
        FloatFunc = d;
    }
}

internal static class Arith
{
    public static readonly Func<Int64, Int64, Int64> Iadd = (a, b) => a + b;
    public static readonly Func<double, double, double> Fadd = (a, b) => a + b;
    public static readonly Func<Int64, Int64, Int64> Isub = (a, b) => a - b;
    public static readonly Func<double, double, double> Fsub = (a, b) => a - b;
    public static readonly Func<Int64, Int64, Int64> Imul = (a, b) => a * b;
    public static readonly Func<double, double, double> Fmul = (a, b) => a * b;
    public static readonly Func<Int64, Int64, Int64> Imod = Number.Mod;
    public static readonly Func<double, double, double> Fmod = Number.Mod;
    public static readonly Func<double, double, double> Pow = Math.Pow;
    public static readonly Func<double, double, double> Div = (a, b) => a / b;
    public static readonly Func<Int64, Int64, Int64> Iidiv = Number.FloorDiv;
    public static readonly Func<double, double, double> Fidiv = Number.FloorDiv;
    public static readonly Func<Int64, Int64, Int64> Band = (a, b) => a & b;
    public static readonly Func<Int64, Int64, Int64> Bor = (a, b) => a | b;
    public static readonly Func<Int64, Int64, Int64> Bxor = (a, b) => a ^ b;
    public static readonly Func<Int64, Int64, Int64> Shl = Number.ShiftLeft;
    public static readonly Func<Int64, Int64, Int64> Shr = Number.ShiftRight;
    public static readonly Func<Int64, Int64, Int64> Iunm = (a, _) => -a;
    public static readonly Func<double, double, double> Funm = (a, _) => -a;
    public static readonly Func<Int64, Int64, Int64> Bnot = (a, _) => ~a;

    public static readonly Operator[] Operators = new Operator[]
    {
        new Operator(Iadd, Fadd),
        new Operator(Isub, Fsub),
        new Operator(Imul, Fmul),
        new Operator(Imod, Fmod),
        new Operator(null, Pow),
        new Operator(null, Div),
        new Operator(Iidiv, Fidiv),
        new Operator(Band, null),
        new Operator(Bor, null),
        new Operator(Bxor, null),
        new Operator(Shl, null),
        new Operator(Shr, null),
        new Operator(Iunm, Funm),
        new Operator(Bnot, null),
    };
}
