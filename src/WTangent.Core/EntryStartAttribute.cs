using System;

namespace WTangent.Components;

/// <summary>标记启动钩子（Entry 类内方法，static/instance 均可，void 或 Task）：
/// 生成器检测是否 async，生成真正的 StartAsync（async 钩子直通返回 Task，sync 包装 CompletedTask）。</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class EntryStartAttribute : Attribute;
