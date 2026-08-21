using System;

namespace WTangent.Components;

/// <summary>标记 LLM 工具实现类（ITool，来自 WTangent.Core）：源生成器收集进组件入口
/// Entry.Tools，serve 启动时加载 tool 类型组件并合并进工具列表。
/// 约定：工具类需无参构造（与 [AgentComponent] 命令类一致）。</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AgentToolAttribute : Attribute;
