using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class FuncDefExp : Exp
    {
        public int Line;
        public int LastLine;
        public List<string> ParList;
        public bool IsVararg;
        public Block Block;
    }
}
