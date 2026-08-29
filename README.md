# WTangent.Components

wtangent 组件体系的**共享底座**：`WTangent.Core`（运行时契约 + 日志/配置/存储/LLM 等默认设施）+ 源生成器（`[AgentEntry]`/`[AgentCommand]`/`[AgentTool]`/`[EntryStart]` → 自动产出 Entry 接线）。

## 引用方式（已去 nuget，纯源码引用）

所有组件仓 / 空壳 csproj 直接 ProjectReference 本仓的**平级目录**（与 `D:\Agent` 工作区布局一致；CI checkout 同布局）：

```xml
<ItemGroup>
  <ProjectReference Include="..\WTangent.Components\src\WTangent.Core\WTangent.Core.csproj" Private="false"/>
  <ProjectReference Include="..\WTangent.Components\src\WTangent.Components\Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" Private="false"/>
</ItemGroup>
```

- `Private="false"`：Core 不拷进组件输出/zip——运行期 Core 由空壳统一提供（单 ALC 简单名统一）
- `WTangentDev=true` 构建时关闭上述引用（第三方 `wtangent dev restore` 的 props 注入模式，HintPath 指向 release 资产）

## 发版

手动触发 Actions 的 release workflow → release-please 管版本（always-bump-patch）→ release 挂两个资产：`WTangent.Core.dll` + `WTangent.Components.dll`（生成器）。这两个 dll 是 `wtangent dev restore` 的直拉通道（组件开发者免工作区、免编译本仓）。

架构细节（加载模型/minCore 门禁/depends/发版顺序）以 `WTangent.Server/AGENTS.md` 为准。
