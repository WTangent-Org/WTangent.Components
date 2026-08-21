using System;

namespace WTangent.Components;

/// <summary>标记组件的顶级行为方法（public static int Method(string[] args)）：
/// 源生成器据此生成完整 Entry 的 Default 属性；未标记则 Default 为 null。</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class AgentDefaultAttribute : Attribute
{
}
