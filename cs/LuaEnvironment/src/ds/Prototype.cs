using System;
using System.Collections.Generic;
using System.Text;

public class Prototype
{
    public static class Tags
    {
        public const byte NIL = 0x00;
        public const byte BOOLEAN = 0x01;
        public const byte NUMBER = 0x03;
        public const byte INTEGER = 0x13;
        public const byte SHORT_STR = 0x04;
        public const byte LONG_STR = 0x14;
    }

    public string Source;
    public UInt32 LineDefined;
    public UInt32 LastLineDefined;
    public byte NumParams;
    public byte IsVararg;
    public byte MaxStackSize;
    public UInt32[] Code;
    public object[] Constants;
    public UpValue[] UpValues;
    public Prototype[] Protos;
    public UInt32[] LineInfo;
    public LocVar[] LocVars;
    public string[] UpvalueNames;

    public Prototype() { }

    public static Prototype Undump(byte[] data)
    {
        Reader reader = new Reader(data);
        reader.CheckHeader(); // 校验头部
        reader.ReadByte(); // 跳过UpValue变量
        return reader.ReadProto(""); // 读取函数原型
    }

    private string HeaderToString()
    {
        StringBuilder builder = new StringBuilder();
        string funcType = "main";
        if (LineDefined > 0)
        {
            funcType = "function";
        }

        string varArgFlag = string.Empty;

        if (IsVararg > 0)
        {
            varArgFlag = "+";
        }

        builder.AppendLine($"\n{funcType} <{Source}:{LineDefined},{LastLineDefined}> ({Code.Length} instructions)");
        builder.AppendLine($"{NumParams}{varArgFlag} params, {MaxStackSize} slots, {UpValues} upvalues, ");
        builder.AppendLine($"{LocVars.Length} locals, {Constants.Length} constants, {Protos.Length} functions");

        return builder.ToString();
    }

    private string CodeToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < Code.Length; i++)
        {
            string line = "-";
            if (LineInfo.Length > 0)
            {
                line = LineInfo[i].ToString();
            }

            Instruction ins = Code[i];
            builder.Append($"\t{i + 1}\t[{line}]\t{ins.OpName} \t");
            builder.Append(PrintOperands(ins));
            builder.AppendLine();
        }

        return builder.ToString();

        string PrintOperands(Instruction i)
        {
            StringBuilder builder = new StringBuilder();
            switch (i.OpFormat)
            {
                case OpFormat.IABC:
                    var (a, b, c) = i.ABC();
                    builder.Append(a.ToString());
                    if (i.BMode != OpArgType.OpArgN)
                    {
                        if (b > 0xff)
                        {
                            builder.Append($" {-1 - (b & 0xff)}");
                        }
                        else
                        {
                            builder.Append($" {b}");
                        }
                    }
                    if (i.CMode != OpArgType.OpArgN)
                    {
                        if (c > 0xff)
                        {
                            builder.Append($" {-1 - (c & 0xff)}");
                        }
                        else
                        {
                            builder.Append($" {c}");
                        }
                    }
                    break;
                case OpFormat.IABx:
                    var (a1, bx) = i.ABx();
                    builder.Append(a1.ToString());
                    if (i.BMode == OpArgType.OpArgK)
                    {
                        builder.Append($" {-1 - bx}");
                    }
                    else if (i.BMode == OpArgType.OpArgU)
                    {
                        builder.Append($" {bx}");
                    }
                    break;
                case OpFormat.IAsBx:
                    var (a2, sbx) = i.AsBx();
                    builder.Append($"{a2} {sbx}");
                    break;
                case OpFormat.IAx:
                    var a3 = i.Ax();
                    builder.Append($"{a3}");
                    break;
                default:
                    builder.Append("???");
                    break;
            }
            return builder.ToString();
        }
    }


    private string DetailToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"constants ({Constants.Length}):");
        for (int i = 0; i < Constants.Length; i++)
        {
            builder.AppendLine($"\t{i + 1}\t{ConstantToString(Constants[i])}");
        }

        builder.AppendLine($"locals ({LocVars.Length}):");
        for (int i = 0; i < LocVars.Length; i++)
        {
            builder.AppendLine($"\t{i + 1}\t{LocVars[i].VarName}\t{LocVars[i].StartPC + 1}\t{LocVars[i].EndPC + 1}");
        }

        builder.AppendLine($"upvalues ({UpValues.Length}):");
        for (int i = 0; i < UpValues.Length; i++)
        {
            builder.AppendLine($"\t{i + 1}\t{UpvalName(i)}\t{(int)UpValues[i].InStack}\t{(int)UpValues[i].Idx}");
        }
        return builder.ToString();
    }

    private string UpvalName(int i)
    {
        if (UpvalueNames.Length > 0)
        {
            return UpvalueNames[i];
        }
        return "-";
    }

    private string ConstantToString(object v)
    {
        switch (v)
        {
            case null:
                return "nil";
            case bool b:
                return b.ToString();
            case double d:
                return d.ToString();
            case Int64 i:
                return i.ToString();
            case string s:
                return s;
            default:
                return "?";
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine(HeaderToString());
        builder.AppendLine(CodeToString());
        builder.AppendLine(DetailToString());

        foreach (var proto in Protos)
        {
            builder.AppendLine(proto.ToString());
        }

        return builder.ToString();
    }
}