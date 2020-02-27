using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class TableAccessExp : Exp
    {
        public int LastLine;
        public Exp PrefixExp;
        public Exp KeyExp;
    }
}
