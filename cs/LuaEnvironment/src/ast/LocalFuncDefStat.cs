using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class LocalFuncDefStat : Stat
    {
        public string Name;
        public FuncDefExp Exp;
    }
}
