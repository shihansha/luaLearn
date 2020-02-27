using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class IfStat : Stat
    {
        public List<Exp> Exps;
        public List<Block> Blocks;
    }
}
