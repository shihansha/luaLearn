using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

internal class Reader
{
    private int index = 0;
    public byte[] Data;

    public Reader(byte[] data)
    {
        Data = data;
    }

    private void Consume(int num)
    {
        index += num;
        Data = Data.Skip(num).Take(Data.Length - num).ToArray();
    }

    public byte ReadByte()
    {
        byte b = Data[0];
        Consume(1);
        return b;
    }

    public UInt32 ReadUint32()
    {
        UInt32 i = BitConverter.ToUInt32(Data, 0);
        Consume(sizeof(UInt32));
        return i;
    }

    public UInt64 ReadUint64()
    {
        UInt64 i = BitConverter.ToUInt64(Data, 0);
        Consume(sizeof(UInt64));
        return i;
    }

    public Int64 ReadLuaInteger()
    {
        Int64 i = BitConverter.ToInt64(Data, 0);
        Consume(sizeof(Int64));
        return i;
    }

    public double ReadLuaNumber()
    {
        double d = BitConverter.ToDouble(Data, 0);
        Consume(sizeof(double));
        return d;
    }

    public string ReadString()
    {
        int size = ReadByte();
        if (size == 0)
        {
            return string.Empty;
        }
        if (size == 0xff)
        {
            size = (int)ReadUint64();
        }

        byte[] bytes = ReadBytes(size - 1);
        return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
    }

    private byte[] ReadBytes(int length)
    {
        byte[] bytes = Data.Take(length).ToArray();
        Consume(length);
        return bytes;
    }

    private void Panic(string str)
    {
        throw new LuaException(str);
    }

    public void CheckHeader()
    {
        Header def = Header.Default;

        var sig = ReadBytes(4);
        for (int i = 0; i < def.Signature.Length; i++)
        {
            if (def.Signature[i] != sig[i])
            {
                Panic("not a precompiled chunk!");
                return;
            }
        }

        if (ReadByte() != def.Version)
        {
            Panic("version mismatch!");
            return;
        }

        if (ReadByte() != def.Format)
        {
            Panic("format dismatch!");
            return;
        }

        var dat = ReadBytes(6);
        for (int i = 0; i < def.LuacData.Length; i++)
        {
            if (def.LuacData[i] != dat[i])
            {
                Console.WriteLine("diff! org: " + dat[i] + ", expected: " + def.LuacData[i]);
                Panic("corrupted!");
                return;
            }
        }

        if (ReadByte() != def.CintState)
        {
            Panic("int size mismatch!");
            return;
        }

        if (ReadByte() != def.SizetSize)
        {
            Panic("size_t size mismatch!");
            return;
        }

        if (ReadByte() != def.InstructionSize)
        {
            Panic("instruction size mismatch!");
            return;
        }

        if (ReadByte() != def.LuaIntegerSize)
        {
            Panic("lua_Integer size mismatch!");
            return;
        }

        if (ReadByte() != def.LuaNumberSize)
        {
            Panic("lua_Number size mismatch!");
            return;
        }

        if (ReadLuaInteger() != def.LuacInt)
        {
            Panic("endianess size mismatch!");
            return;
        }

        if (ReadLuaNumber() != def.LuacNum)
        {
            Panic("float format mismatch!");
            return;
        }
    }

    public Prototype ReadProto(string parentSource)
    {
        string source = ReadString();
        if (string.IsNullOrEmpty(source))
        {
            source = parentSource;
        }
        return new Prototype()
        {
            Source = source,
            LineDefined = ReadUint32(),
            LastLineDefined = ReadUint32(),
            NumParams = ReadByte(),
            IsVararg = ReadByte(),
            MaxStackSize = ReadByte(),
            Code = ReadCode(),
            Constants = ReadConstants(),
            UpValues = ReadUpvalues(),
            Protos = ReadProtos(source),
            LineInfo = ReadLineInfo(),
            LocVars = ReadLocalVars(),
            UpvalueNames = ReadUpvalueNames()
        };
    }

    private string[] ReadUpvalueNames()
    {
        string[] names = new string[ReadUint32()];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = ReadString();
        }
        return names;
    }

    private LocVar[] ReadLocalVars()
    {
        LocVar[] locVars = new LocVar[ReadUint32()];
        for (int i = 0; i < locVars.Length; i++)
        {
            locVars[i] = new LocVar
            {
                VarName = ReadString(),
                StartPC = ReadUint32(),
                EndPC = ReadUint32(),
            };
        }
        return locVars;
    }

    private uint[] ReadLineInfo()
    {
        uint[] lineInfos = new uint[ReadUint32()];
        for (int i = 0; i < lineInfos.Length; i++)
        {
            lineInfos[i] = ReadUint32();
        }
        return lineInfos;
    }

    private Prototype[] ReadProtos(string parentSource)
    {
        Prototype[] protos = new Prototype[ReadUint32()];
        for (int i = 0; i < protos.Length; i++)
        {
            protos[i] = ReadProto(parentSource);
        }
        return protos;
    }

    private UpValue[] ReadUpvalues()
    {
        UpValue[] upValues = new UpValue[ReadUint32()];
        for (int i = 0; i < upValues.Length; i++)
        {
            upValues[i] = new UpValue
            {
                InStack = ReadByte(),
                Idx = ReadByte(),
            };
        }
        return upValues;
    }

    private object[] ReadConstants()
    {
        object[] constants = new object[ReadUint32()];
        for (int i = 0; i < constants.Length; i++)
        {
            constants[i] = ReadConstant();
        }
        return constants;
    }

    private object ReadConstant()
    {
        switch (ReadByte())
        {
            case Prototype.Tags.NIL:
                return null;
            case Prototype.Tags.BOOLEAN:
                return ReadByte() != 0;
            case Prototype.Tags.INTEGER:
                return ReadLuaInteger();
            case Prototype.Tags.NUMBER:
                return ReadLuaNumber();
            case Prototype.Tags.SHORT_STR:
            case Prototype.Tags.LONG_STR:
                return ReadString();
            default:
                Panic("corrupted!");
                return null;
        }
    }

    private uint[] ReadCode()
    {
        uint[] code = new uint[ReadUint32()];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = ReadUint32();
        }
        return code;
    }
}