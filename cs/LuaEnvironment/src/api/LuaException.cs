using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

[Serializable]
public class LuaException : Exception
{
    public LuaValue Content { get; }
    public LuaException()
    {
    }

    public LuaException(LuaValue content) : base(content.ToString())
    {
        Content = content;
    }

    public LuaException(LuaValue content, Exception innerException) : base(content.ToString(), innerException)
    {
        Content = content;
    }

    protected LuaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
