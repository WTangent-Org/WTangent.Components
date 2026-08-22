using System;

namespace WTangent.Components;

/// <summary>标记停止钩子（Entry 类内方法，static/instance 均可，void 或 Task）：
/// 生成器检测是否 async，生成真正的 StopAsync。</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class EntryStopAttribute : Attribute;
