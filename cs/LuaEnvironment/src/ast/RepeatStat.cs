using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class RepeatStat : Stat
    {
        public Block Block;
        public Exp Exp;
    }
}
