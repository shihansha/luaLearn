using System;
using System.Collections.Generic;

internal static partial class VM
{
    internal static void ForPrep(Instruction i, ILuaVM vm)
    {
        var (a, sBx) = i.AsBx();
        a += 1;

        // R(A)-=R(A+2)
        vm.PushValue(a);
        vm.PushValue(a + 2);
        vm.Arith(ArithOp.Sub);
        vm.Replace(a);
        // pc += sBx
        vm.AddPC(sBx);
    }

    internal static void ForLoop(Instruction i, ILuaVM vm)
    {
        var (a, sBx) = i.AsBx();
        a += 1;

        // R(A)+=R(A+2)
        vm.PushValue(a + 2);
        vm.PushValue(a);
        vm.Arith(ArithOp.Add);
        vm.Replace(a);

        // R(A)<?=R(A+1)
        bool isPositiveStep = vm.ToNumber(a + 2) >= 0;
        if ((isPositiveStep && vm.Compare(a, a + 1, CompareOp.Le))
            || (!isPositiveStep && vm.Compare(a + 1, a, CompareOp.Le)))
        {
            vm.AddPC(sBx); // pc+=sBx
            vm.Copy(a, a + 3); // R(A+3)=R(A)
        }
    }
}
