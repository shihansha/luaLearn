using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class AssignStat : Stat
    {
        public int LastLine;
        public List<Exp> VarList;
        public List<Exp> ExpList;
    }
}
