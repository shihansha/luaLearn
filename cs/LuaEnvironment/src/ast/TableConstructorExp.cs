using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class TableConstructorExp : Exp
    {
        public int Line;
        public int LastLine;
        public List<Exp> KeyExps;
        public List<Exp> ValExps;
    }
}
