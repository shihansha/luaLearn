using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class IntegerExp : Exp
    {
        public int Line;
        public Int64 Val;
    }
}
