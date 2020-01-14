
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

