using System;

public interface ILuaVM : ILuaState
{
    int PC { get; } // 返回当前PC(仅测试用)
    void AddPC(int n); // 修改PC(用于实现跳转指令)
    UInt32 Fetch(); // 取出当前指令; 将PC指向下一条指令
    void GetConst(int idx); // 将指定常量压入栈顶
    void GetRK(int rk); // 将指定常量或栈值压入栈顶
}