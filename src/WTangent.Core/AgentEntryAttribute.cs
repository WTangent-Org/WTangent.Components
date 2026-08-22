using System;

namespace WTangent.Components;

/// <summary>标记组件入口类（IEntry 实现）：源生成器填充 partial Entry 的
/// Identifier / Name / SupportAsyncStart。全部位置参数：[AgentEntry("serve", "serve 服务", false)]。</summary>
/// <param name="identifier">组件标识（= components.json 索引别名）</param>
/// <param name="name">组件显示名</param>
/// <param name="isAsync">是否支持异步启动（true = StartAsync 可与其他组件并行）</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AgentEntryAttribute(string identifier, string name, bool isAsync) : Attribute
{
    /// <summary>组件标识</summary>
    public string Identifier => identifier;

    /// <summary>组件显示名</summary>
    public string Name => name;

    /// <summary>是否支持异步启动</summary>
    public bool IsAsync => isAsync;
}
