using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class BinopExp : Exp
    {
        public int Line;
        public TokenType Op;
        public Exp Exp1;
        public Exp Exp2;
    }
}
