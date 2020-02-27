using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class FuncCallStat : Stat
    {
        public int Line;
        public int LastLine;
        public Exp PrefixExp;
        public StringExp NameExp;
        public List<Exp> Args;
    }
}
