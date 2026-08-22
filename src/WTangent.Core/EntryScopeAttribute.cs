using System;

namespace WTangent.Components;

/// <summary>声明组件入口的作用域（事件键/服务命名前缀）。源生成器填充 partial Entry 的 Scope 属性；
/// 不写则 Scope = Identifier。</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class EntryScopeAttribute(string scope) : Attribute
{
    /// <summary>作用域名</summary>
    public string Scope => scope;
}
