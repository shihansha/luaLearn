using System;
using System.Collections.Generic;

internal static partial class VM
{
    internal static void Move(Instruction i, ILuaVM vm)
    {
        var (a, b, _) = i.ABC();
        a += 1;
        b += 1;
        vm.Copy(b, a);
    }

    internal static void Jmp(Instruction i, ILuaVM vm)
    {
        var (a, sbx) = i.AsBx();
        vm.AddPC(sbx);
        if (a != 0)
        {
            vm.CloseUpvalues(a);
        }
    }
}
