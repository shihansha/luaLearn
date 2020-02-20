
public enum LuaType
{
    None = -1,
    Nil,
    Boolean,
    LightUserData,
    Number,
    String,
    Table,
    Function,
    UserData,
    Thread
}

public enum ArithOp
{
    Add = 0, // +
    Sub, // -
    Mul, // *
    Mod, // %
    Pow, // ^
    Div, // /
    IDiv, // //
    And, // &
    Or, // |
    Xor, // ~
    Shl, // <<
    Shr, // >>
    Unm, // - (unary minus)
    Bnot // ~
}

public enum CompareOp
{
    Eq = 0, // ==
    Lt, // <
    Le, // <=
}

public static class Consts
{
    public const int LUA_MINSTACK = 20;
    public const int LUAI_MAXSTACK = 1_000_000;
    public const int LUA_REGISTRYINDEX = -LUAI_MAXSTACK - 1000;
    public const long LUA_RIDX_GLOBALS = 2;
}

public enum ErrState
{
    Ok = 0,
    Yield,
    ErrRun,
    ErrSyntax,
    ErrMem,
    ErrGCMM,
    ErrErr,
    ErrFile
}


