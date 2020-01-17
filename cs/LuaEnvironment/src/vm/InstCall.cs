using System;
using System.Collections.Generic;
using System.Text;

internal static partial class VM
{
    internal static void Closure(Instruction i, ILuaVM vm)
    {
        var (a, bx) = i.ABx();
        a += 1;

        vm.LoadProto(bx);
        vm.Replace(a);
    }

    internal static void Call(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;

        int nArgs = PushFuncAndArgs(a, b, vm);
        vm.Call(nArgs, c - 1);
        PopResults(a, c, vm);
    }

    private static void PopResults(int a, int c, ILuaVM vm)
    {
        if (c == 1)
        {

        }
        else if (c > 1)
        {
            for (int i = a + c - 2; i >= a; i--)
            {
                vm.Replace(i);
            }
        }
        else
        {
            vm.CheckStack(1);
            vm.PushInteger((Int64)a);
        }
    }

    private static int PushFuncAndArgs(int a, int b, ILuaVM vm)
    {
        if (b >= 1)
        {
            vm.CheckStack(b);
            for (int i = a; i < a + b; i++)
            {
                vm.PushValue(i);
            }
            return b - 1;
        }
        else
        {
            FixStack(a, vm);
            return vm.GetTop() - vm.RegisterCount() - 1;
        }
    }

    private static void FixStack(int a, ILuaVM vm)
    {
        int x = (int)vm.ToInteger(-1);
        vm.Pop(1);

        vm.CheckStack(x - a);
        for (int i = a; i < x; i++)
        {
            vm.PushValue(i);
        }
        vm.Rotate(vm.RegisterCount() + 1, x - a);
    }

    public static void Return(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;

        if (b == 1)
        {

        }
        else if (b > 1)
        {
            vm.CheckStack(b - 1);
            for (int j = a; j <= a + b - 2; j++)
            {
                vm.PushValue(j);
            }
        }
        else
        {
            FixStack(a, vm);
        }
    }

    internal static void Vararg(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;

        if (b != 1)
        {
            vm.LoadVararg(b - 1);
            PopResults(a, b, vm);
        }
    }

    internal static void TailCall(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        int c = 0;

        int nArgs = PushFuncAndArgs(a, b, vm);
        vm.Call(nArgs, c - 1);
        PopResults(a, c, vm);
    }

    internal static void Self(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        c += 1;

        vm.Copy(b, a + 1);
        vm.GetRK(c);
        vm.GetTable(b);
        vm.Replace(a);
    }
}

