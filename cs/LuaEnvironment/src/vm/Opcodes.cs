using System;

internal enum OpFormat
{
    IABC = 0,
    IABx,
    IAsBx,
    IAx,
}

internal enum Opcodes
{
    OP_MOVE = 0,
    OP_LOADK,
    OP_LOADKX,
    OP_LOADBOOL,
    OP_LOADNIL,
    OP_GETUPVAL,
    OP_GETTABUP,
    OP_GETTABLE,
    OP_SETTABUP,
    OP_SETUPVAL,
    OP_SETTABLE,
    OP_NEWTABLE,
    OP_SELF,
    OP_ADD,
    OP_SUB,
    OP_MUL,
    OP_MOD,
    OP_POW,
    OP_DIV,
    OP_IDIV,
    OP_BAND,
    OP_BOR,
    OP_BXOR,
    OP_SHL,
    OP_SHR,
    OP_UNM,
    OP_BNOT,
    OP_NOT,
    OP_LEN,
    OP_CONCAT,
    OP_JMP,
    OP_EQ,
    OP_LT,
    OP_LE,
    OP_TEST,
    OP_TESTSET,
    OP_CALL,
    OP_TAILCALL,
    OP_RETURN,
    OP_FORLOOP,
    OP_FORPREP,
    OP_TFORCALL,
    OP_TFORLOOP,
    OP_SETLIST,
    OP_CLOSURE,
    OP_VARARG,
    OP_EXTRAARG,
}

internal enum OpArgType
{
    OpArgN = 0, // not used
    OpArgU, // used
    OpArgR, // register or jump offset
    OpArgK, // constant or register/constant
}

internal class OpCodeInfo
{
    public byte TestFlag; // operation is a test (next instruction must be a jump)
    public byte SetAFFlag; // instruction set register A
    public OpArgType ArgBMode; // B arg mode
    public OpArgType ArgCMode; // C arg mode
    public OpFormat OpMode; // op mode
    public string Name;
    public Action<Instruction, ILuaVM> Action;

    public OpCodeInfo(byte t, byte a, OpArgType b, OpArgType c, OpFormat mode, string name, Action<Instruction, ILuaVM> action)
    {
        TestFlag = t;
        SetAFFlag = a;
        ArgBMode = b;
        ArgCMode = c;
        OpMode = mode;
        Name = name;
        Action = action;
    }

    public readonly static OpCodeInfo[] OpCodeInfos = new OpCodeInfo[]
    {
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "MOVE    ", VM.Move),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgN, OpFormat.IABx, "LOADK   ", VM.LoadK),
        new OpCodeInfo(0, 1, OpArgType.OpArgN, OpArgType.OpArgN, OpFormat.IABx, "LOADKX  ", VM.LoadKx),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "LOADBOOL", VM.LoadBool),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "LOADNIL ", VM.LoadNil),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "GETUPVAL", VM.GetUpval),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgK, OpFormat.IABC, "GETTABUP", VM.GetTabUp),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgK, OpFormat.IABC, "GETTABLE", VM.GetTable),
        new OpCodeInfo(0, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SETTABUP", VM.SetTabUp),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "SETUPVAL", VM.SetUpval),
        new OpCodeInfo(0, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SETTABLE", VM.SetTable),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "NEWTABLE", VM.NewTable),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgK, OpFormat.IABC, "SELF    ", VM.Self),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "ADD     ", VM.Add),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SUB     ", VM.Sub),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "MUL     ", VM.Mul),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "MOD     ", VM.Mod),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "POW     ", VM.Pow),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "DIV     ", VM.Div),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "IDIV    ", VM.Idiv),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BAND    ", VM.Band),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BOR     ", VM.Bor),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BXOR    ", VM.Bxor),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SHL     ", VM.Shl),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SHR     ", VM.Shr),

        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "UNM     ", VM.Unm),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "BNOT    ", VM.Bnot),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "NOT     ", VM.Not),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "LEN     ", VM.Len),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgR, OpFormat.IABC, "CONCAT  ", VM.Concat),

        new OpCodeInfo(0, 0, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "JMP     ", VM.Jmp),

        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "EQ      ", VM.Eq),
        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "LT      ", VM.Lt),
        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "LE      ", VM.Le),
        new OpCodeInfo(1, 0, OpArgType.OpArgN, OpArgType.OpArgU, OpFormat.IABC, "TEST    ", VM.Test),
        new OpCodeInfo(1, 1, OpArgType.OpArgR, OpArgType.OpArgU, OpFormat.IABC, "TESTSET ", VM.TestSet),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "CALL    ", VM.Call),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "TAILCALL", VM.TailCall),

        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "RETURN  ", VM.Return),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "FORLOOP ", VM.ForLoop),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "FORPREP ", VM.ForPrep),
        new OpCodeInfo(0, 0, OpArgType.OpArgN, OpArgType.OpArgU, OpFormat.IABC, "TFORCALL", null),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "TFORLOOP", null),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "SETLIST ", VM.SetList),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABx, "CLOSURE ", VM.Closure),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "VARARG  ", VM.Vararg),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IAx, "EXTRAARG", null),
    };
}
