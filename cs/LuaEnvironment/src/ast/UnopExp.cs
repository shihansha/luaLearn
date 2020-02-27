using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class UnopExp : Exp
    {
        public int Line;
        public TokenType Op;
        public Exp Exp;
    }
}
