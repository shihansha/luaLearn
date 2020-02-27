using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class StringExp : Exp
    {
        public int Line;
        public string Str;
    }
}
