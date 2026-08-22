using System;

namespace WTangent.Components;

/// <summary>标记组件命令类（继承 System.CommandLine.Command 的类）：
/// 源生成器收集后生成 partial Entry 的 Commands 属性（编译期，无需手写命令数组）。
/// parent 写明挂载位置（含根名 root，如 "root/remote"）；null = 顶级命令（挂根）。</summary>
/// <param name="parent">父路径（含根名 root）；null = 顶级</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AgentCommandAttribute(string? parent = null) : Attribute
{
    /// <summary>父路径</summary>
    public string? Parent => parent;
}
