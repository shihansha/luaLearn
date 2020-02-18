using System;
using System.Collections.Generic;

internal class Operator
{
    public readonly string Metamethod;
    public readonly Func<Int64, Int64, Int64> IntegerFunc;
    public readonly Func<double, double, double> FloatFunc;
    public Operator(string metamethod, Func<Int64, Int64, Int64> i, Func<double, double, double> d)
    {
        Metamethod = metamethod;
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
        new Operator("__add", Iadd, Fadd),
        new Operator("__sub", Isub, Fsub),
        new Operator("__mul", Imul, Fmul),
        new Operator("__mod", Imod, Fmod),
        new Operator("__pow", null, Pow),
        new Operator("__div", null, Div),
        new Operator("__idiv", Iidiv, Fidiv),
        new Operator("__band", Band, null),
        new Operator("__bor", Bor, null),
        new Operator("__bxor", Bxor, null),
        new Operator("__shl", Shl, null),
        new Operator("__shr", Shr, null),
        new Operator("__unm", Iunm, Funm),
        new Operator("__bnot", Bnot, null),
    };
}
