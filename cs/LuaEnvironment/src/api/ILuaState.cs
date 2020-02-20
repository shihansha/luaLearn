using System;
using System.Collections.Generic;

public delegate int CSharpFunction(ILuaState luaState);

public interface ILuaState
{
    // basic stack manipulation
    int GetTop();
    int AbsIndex(int idx);
    bool CheckStack(int n);
    void Pop(int n);
    void Copy(int fromIdx, int toIdx);
    void PushValue(int idx);
    void Replace(int idx);
    void Insert(int idx);
    void Remove(int idx);
    void Rotate(int idx, int n);
    void SetTop(int idx);
    // access functions (stack -> c#)
    string TypeName(LuaType tp);
    LuaType Type(int idx);
    bool IsNone(int idx);
    bool IsNil(int idx);
    bool IsNoneOrNil(int idx);
    bool IsBoolean(int idx);
    bool IsInteger(int idx);
    bool IsNumber(int idx);
    bool IsString(int idx);
    bool ToBoolean(int idx);
    Int64 ToInteger(int idx);
    (Int64, bool) ToIntegerX(int idx);
    double ToNumber(int idx);
    (double, bool) ToNumberX(int idx);
    string ToString(int idx);
    (string, bool) ToStringX(int idx);
    // push functions (c# -> stack)
    void PushNil();
    void PushBoolean(bool b);
    void PushInteger(Int64 n);
    void PushNumber(double n);
    void PushString(string s);
    // math functions
    void Arith(ArithOp op);
    bool Compare(int idx1, int idx2, CompareOp op);
    void Len(int idx);
    void Concat(int n);
    // table functions
    // get functions
    void NewTable();
    void CreateTable(int nArr, int nRec);
    LuaType GetTable(int idx);
    LuaType GetField(int idx, string k);
    LuaType GetI(int idx, Int64 i);
    // set functions
    void SetTable(int idx);
    void SetField(int idx, string k);
    void SetI(int idx, Int64 n);
    // exec
    ErrState Load(byte[] chunk, string chunkName, string mode);
    void Call(int nArgs, int nResults);
    // csharp func call
    void PushCSharpFunction(CSharpFunction cs);
    bool IsCSharpFunction(int idx);
    CSharpFunction ToCSharpFunction(int idx);

    LuaTable Registry { get; }
    LuaStack LuaStack { get; }

    void PushGlobalTable();
    LuaType GetGlobal(string name);
    void SetGlobal(string name);
    void Register(string name, CSharpFunction cs);

    void PushCSharpClosure(CSharpFunction f, int n);

    bool GetMetatable(int idx);
    void SetMetatable(int idx);
    uint RawLen(int idx);
    bool RawEqual(int idx1, int idx2);
    LuaType RawGet(int idx);
    void RawSet(int idx);
    LuaType RawGetI(int idx, Int64 i);
    void RawSetI(int idx, Int64 i);
    LuaType RawGetGlobal(string name);
    void RawSetGlobal(string name);
    bool Next(int idx);

    int Error();
    ErrState PCall(int nArgs, int nResults, int msgh);
}