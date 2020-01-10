using System;

internal class Instruction
{
    public UInt32 Data;


    private const int MAXARG_Bx = (1 << 18) - 1;
    private const int MAXARG_sBx = MAXARG_Bx >> 1;

    public Instruction(UInt32 data)
    {
        Data = data;
    }

    public static implicit operator Instruction(UInt32 data)
    {
        return new Instruction(data);
    }

    public int Opcode => (int)(Data & 0x3f);

    public (int, int, int) ABC()
    {
        int a = (int)((Data >> 6) & 0xff);
        int c = (int)((Data >> 14) & 0x1ff);
        int b = (int)((Data >> 23) & 0x1ff);
        return (a, b, c);
    }

    public (int, int) ABx()
    {
        int a = (int)((Data >> 6) & 0xff);
        int bx = (int)((Data >> 14));
        return (a, bx);
    }

    public (int, int) AsBx()
    {
        var (a, bx) = ABx();
        return (a, bx - MAXARG_sBx);
    }

    public int Ax()
    {
        return (int)(Data >> 6);
    }

    public string OpName => OpCodeInfo.OpCodeInfos[Opcode].Name;
    public OpFormat OpFormat => OpCodeInfo.OpCodeInfos[Opcode].OpMode;
    public OpArgType BMode => OpCodeInfo.OpCodeInfos[Opcode].ArgBMode;
    public OpArgType CMode => OpCodeInfo.OpCodeInfos[Opcode].ArgCMode;

}
