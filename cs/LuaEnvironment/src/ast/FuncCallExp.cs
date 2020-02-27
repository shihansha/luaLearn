using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class FuncCallExp : Exp
    {
        public int Line;
        public int LastLine;
        public Exp PrefixExp;
        public StringExp NameExp;
        public List<Exp> Args;
    }
}
