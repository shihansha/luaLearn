﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LuaEnvironment.src.ast
{
    public class LocalVarDeclStat : Stat
    {
        public int LastLine;
        public List<string> NameList;
        public List<Exp> ExpList;
    }
}
