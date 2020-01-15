using System;
using System.Collections.Generic;

internal static partial class VM
{
    internal static void LoadNil(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;

        vm.PushNil();
        for (int j = a; j <= a + b; j++)
        {
            vm.Copy(-1, j);
        }
        vm.Pop(1);
    }

    internal static void LoadBool(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        vm.PushBoolean(b != 0);
        vm.Replace(a);
        if (c != 0)
        {
            vm.AddPC(1);
        }
    }

    internal static void LoadK(Instruction i, ILuaVM vm)
    {
        var (a, bx) = i.ABx();
        a += 1;
        vm.GetConst(bx);
        vm.Replace(a);
    }

    internal static void LoadKx(Instruction i, ILuaVM vm)
    {
        var (a, _) = i.ABx();
        a += 1;
        int ax = ((Instruction)vm.Fetch()).Ax();

        vm.GetConst(ax);
        vm.Replace(a);
    }
}
