using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class Block
    {
        public int LastLine;
        public List<Stat> Stats;
        public List<Exp> RetExps;
    }
}
