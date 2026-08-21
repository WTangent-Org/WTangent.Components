# WtAgent.Components

wtagent 组件**共享源生成器**（只做生成器——共享设施在 [WtAgent.Core](https://github.com/wtommy932/WtAgent.Core)）。

## 用法

```xml
<PackageReference Include="WtAgent.Components" Version="0.3.x" PrivateAssets="all" />
<Using Include="WtAgent.Components" />
```

## 特性

- `[AgentComponent]`：标记 System.CommandLine `Command` 子类 → 编译期自动收集
- `[AgentDefault]`：标记组件顶级行为方法（`static int Method(string[] args)`）

## 生成结果（Entry 完整生成，组件不再手写）

```csharp
// 组件里只写命令类：
[AgentComponent]
public sealed class ServeCommand : Command { ... }

// 生成器自动产出（{RootNamespace}.Entry）：
public static class Entry
{
    public static System.CommandLine.Command[] Commands { get; } = [new ServeCommand(), ...];
    public static System.Func<string[], int>? Default => ...;   // [AgentDefault] 或 null
}
```

新增命令 = 标个特性，**零手写注册**。

## 发布

手动发版（Actions 页 run release workflow，可填版本号）→ Trusted Publishing 发布 nuget.org（无 API key）。
