using System;
using System.Collections.Generic;
using System.Linq;

public class Header
{
    public byte[] Signature;
    public byte Version;
    public byte Format;
    public byte[] LuacData;
    public byte CintState;
    public byte SizetSize;
    public byte InstructionSize;
    public byte LuaIntegerSize;
    public byte LuaNumberSize;
    public Int64 LuacInt;
    public double LuacNum;

    private static Header _default;
    public static Header Default
    {
        get 
        {
            if (_default == null)
            {
                _default = new Header()
                {
                    // Signature = System.Text.Encoding.ASCII.GetBytes("\x1bLua"),
                    Signature = "\x1bLua".Select(c => (byte)c).ToArray(),
                    Version = 0x53,
                    Format = 0,
                    LuacData = "\x19\x93\r\n\x1a\n".Select(c => (byte)c).ToArray(),
                    CintState = 4,
                    SizetSize = 4,
                    InstructionSize = 4,
                    LuaIntegerSize = 8,
                    LuaNumberSize = 8,
                    LuacInt = 0x5678,
                    LuacNum = 370.5
                };
            }
            return _default;
        }
    }
}
