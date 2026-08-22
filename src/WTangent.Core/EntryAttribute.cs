using System;

namespace WTangent.Components;

/// <summary>标记组件入口类（IEntry 实现）：源生成器填充 partial Entry 的
/// Identifier（RootNamespace 末段小写，id 可覆盖）/ Name（=Identifier）/ SupportAsyncStart。</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class EntryAttribute(bool isAsync = false, string? id = null) : Attribute
{
    /// <summary>是否支持异步启动（true = StartAsync 可与其他组件并行）</summary>
    public bool IsAsync => isAsync;

    /// <summary>组件标识覆盖（缺省 = RootNamespace 末段小写，如 WTangent.Server → serve）</summary>
    public string? Id => id;
}
