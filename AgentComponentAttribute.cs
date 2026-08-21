using System;

namespace WTangent.Components;

/// <summary>标记组件命令类（System.CommandLine Command 子类）：
/// 源生成器收集后自动生成 partial Entry 的 Commands 属性（编译期，无需手写命令数组）。</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AgentComponentAttribute : Attribute
{
    /// <summary>可选命令名覆盖（缺省用类名去 Command 后缀）</summary>
    public string? Name { get; set; }
}
