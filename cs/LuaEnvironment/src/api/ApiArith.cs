using System;
using System.Collections.Generic;

public partial class LuaState
{
    public void Arith(ArithOp op)
    {
        LuaValue a, b;
        b = LuaStack.Pop();

        if (op != ArithOp.Unm && op != ArithOp.Bnot)
        {
            a = LuaStack.Pop();
        }
        else
        {
            a = b;
        }

        var aop = global::Arith.Operators[(int)op];
        var result = ArithSub(a, b, aop);
        if (result.Value != null)
        {
            LuaStack.Push(result);
            return;
        }

        string mm = aop.Metamethod;
        if (LuaValue.TryCallMetamethod(a, b, mm, this, out LuaValue metaRes))
        {
            LuaStack.Push(metaRes);
            return;
        }

        throw new Exception("arithmetic error!");

        static LuaValue ArithSub(LuaValue a, LuaValue b, Operator op)
        {
            if (op.FloatFunc == null) // bitwise
            {
                var (x, ok) = a.ToInteger();
                if (ok)
                {
                    var (y, ok2) = b.ToInteger();
                    if (ok2)
                    {
                        return new LuaValue(op.IntegerFunc.Invoke(x, y));
                    }
                }
            }
            else
            {
                if (op.IntegerFunc != null) // add,sub,mul,mod,idiv,unm
                {
                    if (a.Value is Int64 x)
                    {
                        if (b.Value is Int64 y)
                        {
                            return new LuaValue(op.IntegerFunc(x, y));
                        }
                    }
                }
                // call float function
                var (x1, ok) = a.ToFloat();
                if (ok)
                {
                    var (y1, ok2) = b.ToFloat();
                    if (ok2)
                    {
                        return new LuaValue(op.FloatFunc.Invoke(x1, y1));
                    }
                }
            }

            return new LuaValue(null);
        }
    }
}
