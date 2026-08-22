using System;

namespace WTangent.Components;

/// <summary>标记事件处理方法（static void 方法，参数 object? payload）：
/// 源生成器收集进 partial Entry 的 CollectedSubscribe(IEventBus)，手写 Entry.StartAsync 里
/// 调用 CollectedSubscribe(app.Events) 完成订阅。</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class AgentEventAttribute(string key) : Attribute
{
    /// <summary>事件键（如 "store.remotes.changed"）</summary>
    public string Key => key;
}
