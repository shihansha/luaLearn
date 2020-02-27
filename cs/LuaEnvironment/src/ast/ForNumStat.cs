using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class ForNumStat : Stat
    {
        public int LineOfFor;
        public int LineOfDo;
        public string VarName;
        public Exp InitExp;
        public Exp LimitExp;
        public Exp StepExp;
        public Block Block;
    }
}
