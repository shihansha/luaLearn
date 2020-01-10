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

    public OpCodeInfo(byte t, byte a, OpArgType b, OpArgType c, OpFormat mode, string name)
    {
        TestFlag = t;
        SetAFFlag = a;
        ArgBMode = b;
        ArgCMode = c;
        OpMode = mode;
        Name = name;
    }

    public readonly static OpCodeInfo[] OpCodeInfos = new OpCodeInfo[]
    {
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "MOVE    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgN, OpFormat.IABx, "LOADK   "),
        new OpCodeInfo(0, 1, OpArgType.OpArgN, OpArgType.OpArgN, OpFormat.IABx, "LOADKX  "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "LOADBOOL"),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "LOADNIL "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "GETUPVAL"),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgK, OpFormat.IABC, "GETTABUP"),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgK, OpFormat.IABC, "GETTABLE"),
        new OpCodeInfo(0, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SETTABUP"),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "SETUPVAL"),
        new OpCodeInfo(0, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SETTABLE"),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "NEWTABLE"),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgK, OpFormat.IABC, "SELF    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "ADD     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SUB     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "MUL     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "MOD     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "POW     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "DIV     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "IDIV    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BAND    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BOR     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "BXOR    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SHL     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "SHR     "),

        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "UNM     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "BNOT    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "NOT     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IABC, "LEN     "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgR, OpFormat.IABC, "CONCAT  "),

        new OpCodeInfo(0, 0, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "JMP     "),

        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "EQ      "),
        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "LT      "),
        new OpCodeInfo(1, 0, OpArgType.OpArgK, OpArgType.OpArgK, OpFormat.IABC, "LE      "),
        new OpCodeInfo(1, 0, OpArgType.OpArgN, OpArgType.OpArgU, OpFormat.IABC, "TEST    "),
        new OpCodeInfo(1, 1, OpArgType.OpArgR, OpArgType.OpArgU, OpFormat.IABC, "TESTSET "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "CALL    "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "TAILCALL"),

        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "RETURN  "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "FORLOOP "),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "FORPREP "),
        new OpCodeInfo(0, 0, OpArgType.OpArgN, OpArgType.OpArgU, OpFormat.IABC, "TFORCALL"),
        new OpCodeInfo(0, 1, OpArgType.OpArgR, OpArgType.OpArgN, OpFormat.IAsBx, "TFORLOOP"),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IABC, "SETLIST "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABx, "CLOSURE "),
        new OpCodeInfo(0, 1, OpArgType.OpArgU, OpArgType.OpArgN, OpFormat.IABC, "VARARG  "),
        new OpCodeInfo(0, 0, OpArgType.OpArgU, OpArgType.OpArgU, OpFormat.IAx, "EXTRAARG"),
    };
}
