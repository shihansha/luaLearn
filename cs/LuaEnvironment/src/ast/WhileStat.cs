using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class WhileStat : Stat
    {
        public Exp Exp;
        public Block Block;
    }
}
