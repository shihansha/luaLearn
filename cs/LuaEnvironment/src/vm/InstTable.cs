using System;
using System.Collections.Generic;
using System.Text;

internal static partial class VM
{
    internal static void NewTable(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;

        vm.CreateTable(Fb2int(b), Fb2int(c));
        vm.Replace(a);
    }

    /*
     *  converts an integer to a "floating point byte", represented as
     *  (eeeeexxx), where the real value is (1xxx) * 2^(eeeee - 1) if
     *  eeeee != 0 and (xxx) otherwise.
     */
    private static int Int2fb(int x)
    {
        int e = 0;
        if (x < 8) return x;
        while (x >= (8 << 4))
        {
            x = (x + 0xf) >> 4;
            e += 4;
        }
        while (x >= (8 << 1))
        {
            x = (x + 1) >> 1;
            e++;
        }
        return ((e + 1) << 3) | (x - 8);
    }

    private static int Fb2int(int x)
    {
        if (x < 8)
        {
            return x;
        }
        else
        {
            return ((x & 7) + 8) << ((x >> 3) - 1);
        }
    }

    public static void GetTable(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;
        b += 1;

        vm.GetRK(c);
        vm.GetTable(b);
        vm.Replace(a);
    }

    public static void SetTable(Instruction i, ILuaVM vm)
    {
        var (a, b, c) = i.ABC();
        a += 1;

        vm.GetRK(b);
        vm.GetRK(c);
        vm.SetTable(a);
    }

    public static void SetList(Instruction i, ILuaVM vm)
    {
        const int FIELDS_PER_FLUSH = 50;
        var (a, b, c) = i.ABC();
        a += 1;

        bool bIsZero = b == 0;
        if (bIsZero)
        {
            b = (int)vm.ToInteger(-1) - a - 1;
            vm.Pop(1);
        }

        if (c > 0)
        {
            c -= 1;
        }
        else
        {
            c = ((Instruction)vm.Fetch()).Ax();
        }

        Int64 idx = (Int64)(c * FIELDS_PER_FLUSH);
        for (int j = 1; j <= b; j++)
        {
            idx++;
            vm.PushValue(a + j);
            vm.SetI(a, idx);
        }

        if (bIsZero)
        {
            for (int j = vm.RegisterCount() + 1; j <= vm.GetTop(); j++)
            {
                idx++;
                vm.PushValue(j);
                vm.SetI(a, idx);
            }
            vm.SetTop(vm.RegisterCount());
        }
    }
}
