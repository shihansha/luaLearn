using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class ConcatExp : Exp
    {
        public int Line;
        public List<Exp> Exps;
    }
}
