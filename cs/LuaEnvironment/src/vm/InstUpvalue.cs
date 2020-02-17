using System;
using System.Collections.Generic;
using System.Text;

internal static partial class VM
{
    internal static void GetUpval(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;

        vm.Copy(LuaState.LuaUpvalueIndex(b), a);
    }

    internal static void SetUpval(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;

        vm.Copy(a, LuaState.LuaUpvalueIndex(b));
    }

    internal static void GetTabUp(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        b += 1;

        vm.GetRK(c);
        vm.GetTable(LuaState.LuaUpvalueIndex(b));
        vm.Replace(a);
    }

    internal static void SetTabUp(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;

        vm.GetRK(b);
        vm.GetRK(c);
        vm.SetTable(LuaState.LuaUpvalueIndex(a));
    }
}
