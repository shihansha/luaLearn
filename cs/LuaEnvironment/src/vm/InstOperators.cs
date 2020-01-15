using System;
using System.Collections.Generic;

internal static partial class VM
{
    private static void BinaryArith(Instruction i, ILuaVM vm, ArithOp op)
    {
        var (a, b, c) = i.ABC();
        a += 1;

        vm.GetRK(b);
        vm.GetRK(c);
        vm.Arith(op);
        vm.Replace(a);
    }

    private static void UnaryArith(Instruction i, ILuaVM vm, ArithOp op)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;

        vm.PushValue(b);
        vm.Arith(op);
        vm.Replace(a);
    }

    internal static void Add(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Add);

    internal static void Sub(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Sub);

    internal static void Mul(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Mul);

    internal static void Mod(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Mod);

    internal static void Pow(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Pow);

    internal static void Div(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Div);

    internal static void Idiv(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.IDiv);

    internal static void Band(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.And);

    internal static void Bor(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Or);

    internal static void Bxor(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Xor);

    internal static void Shl(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Shl);

    internal static void Shr(Instruction i, ILuaVM vm) => BinaryArith(i, vm, ArithOp.Shr);

    internal static void Unm(Instruction i, ILuaVM vm) => UnaryArith(i, vm, ArithOp.Unm);

    internal static void Bnot(Instruction i, ILuaVM vm) => UnaryArith(i, vm, ArithOp.Bnot);

    internal static void Len(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;

        vm.Len(b);
        vm.Replace(a);
    }

    internal static void Concat(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        b += 1;
        c += 1;

        int n = c - b + 1;
        vm.CheckStack(n);
        for (int j = b; j <= c; j++)
        {
            vm.PushValue(j);
        }
        vm.Concat(n);
        vm.Replace(a);
    }

    private static void Compare(Instruction i, ILuaVM vm, CompareOp op)
    {
        var (a, b, c) = i.ABC();

        vm.GetRK(b);
        vm.GetRK(c);
        if (vm.Compare(-2, -1, op) != (a != 0))
        {
            vm.AddPC(1);
        }
        vm.Pop(2);
    }

    internal static void Eq(Instruction i, ILuaVM vm) => Compare(i, vm, CompareOp.Eq);

    internal static void Lt(Instruction i, ILuaVM vm) => Compare(i, vm, CompareOp.Lt);

    internal static void Le(Instruction i, ILuaVM vm) => Compare(i, vm, CompareOp.Le);

    internal static void Not(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;

        vm.PushBoolean(!vm.ToBoolean(b));
        vm.Replace(a);
    }

    internal static void TestSet(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        b += 1;

        if (vm.ToBoolean(b) == (c != 0))
        {
            vm.Copy(b, a);
        }
        else
        {
            vm.AddPC(1);
        }
    }

    internal static void Test(Instruction i, ILuaVM vm)
    {
        var (a, _, c) = i.ABC();
        a += 1;

        if (vm.ToBoolean(a) != (c != 0))
        {
            vm.AddPC(1);
        }
    }
}
