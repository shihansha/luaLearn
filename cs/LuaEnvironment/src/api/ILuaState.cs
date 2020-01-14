using System;
using System.Collections.Generic;

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
}