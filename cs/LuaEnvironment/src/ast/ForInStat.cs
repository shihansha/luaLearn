using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class ForInStat : Stat
    {
        public int LineOfDo;
        public List<string> NameList;
        public List<Exp> ExpList;
        public Block Block;
    }
}
