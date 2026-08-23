# Changelog

## [0.0.10](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.9...v0.0.10) (2026-08-23)


### ⚠ BREAKING CHANGES

* Application 移除 Logger/Config（日志/配置统一走全局门面 Log/Config）；revert --skip-duplicate（会掩盖真实推送错误）

### Features

* Application 移除 Logger/Config（日志/配置统一走全局门面 Log/Config）；revert --skip-duplicate（会掩盖真实推送错误） ([333dd7a](https://github.com/WTangent-Org/WTangent.Components/commit/333dd7aba7d950372a2ca498527fee800c5406a8))

## [0.0.9](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.8...v0.0.9) (2026-08-23)


### Features

* App 静态属性构造注入（PCL-CE 式）：生成器产 static App + ctor，IEntry 移除 App 成员 ([318d122](https://github.com/WTangent-Org/WTangent.Components/commit/318d122040f55fe1aad244ac92645f881ea2309b))
* Core 全局门面 Log/Config + 生成器构建时产出 agent-component.json（minCore 版本门禁） ([4d1eadd](https://github.com/WTangent-Org/WTangent.Components/commit/4d1eadd9f85ed622f6e5a6a9d3bfc28f99d8ac67))
* 构造注入 App（无 null!）+ Current 静态桥（PCL-CE 式）；钩子实例方法，纯业务 ([668943a](https://github.com/WTangent-Org/WTangent.Components/commit/668943afb63615a60737078df1077bcaf8a504f6))

## [0.0.8](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.7...v0.0.8) (2026-08-22)


### Features

* Entry 元数据/生命周期特性（[Entry]/[EntryScope]/[EntryStart]/[EntryStop]）+ 命令/工具直接生成 + 事件订阅接线 ([5c6da02](https://github.com/WTangent-Org/WTangent.Components/commit/5c6da02ecb6c979879297b57b4e358134b26a182))
* IEntry 契约 + 生成器 scope 式注入（0.0.3） ([22bdad7](https://github.com/WTangent-Org/WTangent.Components/commit/22bdad7a017eeaf5d8c19b733b862ae7a3c83849))
* 最终特性集 [AgentEntry(id,name,isAsync)]/[EntryStart]/[EntryStop]/[AgentCommand(parent)]/[AgentTool] ([50a93c5](https://github.com/WTangent-Org/WTangent.Components/commit/50a93c5a69d1ddc41f416a2c47216c95855e8c6c))
* 契约类 WtAgentApp → Application（0.0.2） ([b4a9b51](https://github.com/WTangent-Org/WTangent.Components/commit/b4a9b512e749399b5d6ac0046cf6e8ea24c6f292))
* 手动 dispatch 发版也自动合并 release PR（全自动流程） ([d8e7894](https://github.com/WTangent-Org/WTangent.Components/commit/d8e789455f4c21f88d29b333669646049a5abf2e))
* 拆两个项目（src/WTangent.Core 契约 + src/WTangent.Components 生成器），打包项目收拢单包 0.0.2 ([07bfcad](https://github.com/WTangent-Org/WTangent.Components/commit/07bfcad5502042d72152246ff5ea9732e7abc624))
* 组件三形态标注 + Entry 元数据特性（0.0.5） ([c96c13f](https://github.com/WTangent-Org/WTangent.Components/commit/c96c13fae1b28eecafbb2e3e05064baf7303a725))
* 组件共享包合并——源生成器 + 运行时契约单包 WTangent.Components 0.4.0 ([6881a12](https://github.com/WTangent-Org/WTangent.Components/commit/6881a12f7abe309ae91fa782b3b71a4717319ded))


### Bug Fixes

* initial-version 0.1.0→0.0.1（首个版本从 0.0.1 起步） ([23d7def](https://github.com/WTangent-Org/WTangent.Components/commit/23d7def9677e84c1e6cfbbf8034ae1f51e61c8f9))
* release-please versioning=always-bump-patch（0.0.x 阶段 feat 不再跳 0.1.0）+ extra-files 修正 Generator.csproj ([51ac6a7](https://github.com/WTangent-Org/WTangent.Components/commit/51ac6a7d18c9f4dab54580cbf52b87f146982ae0))
* release.yml 重复头部（startup_failure）；版本 0.0.1 起步 ([733e977](https://github.com/WTangent-Org/WTangent.Components/commit/733e977aceb9099c0860bb7be523f72601d0f4e6))
* 版本回 0.0.2（与已发布包对齐；release-please 0.0.x minor 误推进 0.1.0） ([1206aca](https://github.com/WTangent-Org/WTangent.Components/commit/1206acaba1fe6bc6d78e8eb37716440639dc7820))
* 特性类移入契约项目（lib 编译引用），生成器只留 analyzers——组件 [AgentComponent] 可解析 ([4d8708a](https://github.com/WTangent-Org/WTangent.Components/commit/4d8708a8760d3b6744c11904267eaa3b7210034f))

## [0.0.6](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.5...v0.0.6) (2026-08-22)


### Features

* IEntry 契约 + 生成器 scope 式注入（0.0.3） ([22bdad7](https://github.com/WTangent-Org/WTangent.Components/commit/22bdad7a017eeaf5d8c19b733b862ae7a3c83849))
* 契约类 WtAgentApp → Application（0.0.2） ([b4a9b51](https://github.com/WTangent-Org/WTangent.Components/commit/b4a9b512e749399b5d6ac0046cf6e8ea24c6f292))
* 手动 dispatch 发版也自动合并 release PR（全自动流程） ([d8e7894](https://github.com/WTangent-Org/WTangent.Components/commit/d8e789455f4c21f88d29b333669646049a5abf2e))
* 拆两个项目（src/WTangent.Core 契约 + src/WTangent.Components 生成器），打包项目收拢单包 0.0.2 ([07bfcad](https://github.com/WTangent-Org/WTangent.Components/commit/07bfcad5502042d72152246ff5ea9732e7abc624))
* 组件三形态标注 + Entry 元数据特性（0.0.5） ([c96c13f](https://github.com/WTangent-Org/WTangent.Components/commit/c96c13fae1b28eecafbb2e3e05064baf7303a725))
* 组件共享包合并——源生成器 + 运行时契约单包 WTangent.Components 0.4.0 ([6881a12](https://github.com/WTangent-Org/WTangent.Components/commit/6881a12f7abe309ae91fa782b3b71a4717319ded))


### Bug Fixes

* initial-version 0.1.0→0.0.1（首个版本从 0.0.1 起步） ([23d7def](https://github.com/WTangent-Org/WTangent.Components/commit/23d7def9677e84c1e6cfbbf8034ae1f51e61c8f9))
* release-please versioning=always-bump-patch（0.0.x 阶段 feat 不再跳 0.1.0）+ extra-files 修正 Generator.csproj ([51ac6a7](https://github.com/WTangent-Org/WTangent.Components/commit/51ac6a7d18c9f4dab54580cbf52b87f146982ae0))
* release.yml 重复头部（startup_failure）；版本 0.0.1 起步 ([733e977](https://github.com/WTangent-Org/WTangent.Components/commit/733e977aceb9099c0860bb7be523f72601d0f4e6))
* 版本回 0.0.2（与已发布包对齐；release-please 0.0.x minor 误推进 0.1.0） ([1206aca](https://github.com/WTangent-Org/WTangent.Components/commit/1206acaba1fe6bc6d78e8eb37716440639dc7820))
* 特性类移入契约项目（lib 编译引用），生成器只留 analyzers——组件 [AgentComponent] 可解析 ([4d8708a](https://github.com/WTangent-Org/WTangent.Components/commit/4d8708a8760d3b6744c11904267eaa3b7210034f))

## [0.0.4](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.3...v0.0.4) (2026-08-22)


### Features

* IEntry 契约 + 生成器 scope 式注入（0.0.3） ([22bdad7](https://github.com/WTangent-Org/WTangent.Components/commit/22bdad7a017eeaf5d8c19b733b862ae7a3c83849))
* 契约类 WtAgentApp → Application（0.0.2） ([b4a9b51](https://github.com/WTangent-Org/WTangent.Components/commit/b4a9b512e749399b5d6ac0046cf6e8ea24c6f292))
* 手动 dispatch 发版也自动合并 release PR（全自动流程） ([d8e7894](https://github.com/WTangent-Org/WTangent.Components/commit/d8e789455f4c21f88d29b333669646049a5abf2e))
* 拆两个项目（src/WTangent.Core 契约 + src/WTangent.Components 生成器），打包项目收拢单包 0.0.2 ([07bfcad](https://github.com/WTangent-Org/WTangent.Components/commit/07bfcad5502042d72152246ff5ea9732e7abc624))
* 组件共享包合并——源生成器 + 运行时契约单包 WTangent.Components 0.4.0 ([6881a12](https://github.com/WTangent-Org/WTangent.Components/commit/6881a12f7abe309ae91fa782b3b71a4717319ded))


### Bug Fixes

* initial-version 0.1.0→0.0.1（首个版本从 0.0.1 起步） ([23d7def](https://github.com/WTangent-Org/WTangent.Components/commit/23d7def9677e84c1e6cfbbf8034ae1f51e61c8f9))
* release-please versioning=always-bump-patch（0.0.x 阶段 feat 不再跳 0.1.0）+ extra-files 修正 Generator.csproj ([51ac6a7](https://github.com/WTangent-Org/WTangent.Components/commit/51ac6a7d18c9f4dab54580cbf52b87f146982ae0))
* release.yml 重复头部（startup_failure）；版本 0.0.1 起步 ([733e977](https://github.com/WTangent-Org/WTangent.Components/commit/733e977aceb9099c0860bb7be523f72601d0f4e6))
* 版本回 0.0.2（与已发布包对齐；release-please 0.0.x minor 误推进 0.1.0） ([1206aca](https://github.com/WTangent-Org/WTangent.Components/commit/1206acaba1fe6bc6d78e8eb37716440639dc7820))
* 特性类移入契约项目（lib 编译引用），生成器只留 analyzers——组件 [AgentComponent] 可解析 ([4d8708a](https://github.com/WTangent-Org/WTangent.Components/commit/4d8708a8760d3b6744c11904267eaa3b7210034f))

## [0.1.0](https://github.com/WTangent-Org/WTangent.Components/compare/v0.0.2...v0.1.0) (2026-08-21)


### Features

* 契约类 WtAgentApp → Application（0.0.2） ([b4a9b51](https://github.com/WTangent-Org/WTangent.Components/commit/b4a9b512e749399b5d6ac0046cf6e8ea24c6f292))
* 手动 dispatch 发版也自动合并 release PR（全自动流程） ([d8e7894](https://github.com/WTangent-Org/WTangent.Components/commit/d8e789455f4c21f88d29b333669646049a5abf2e))
* 拆两个项目（src/WTangent.Core 契约 + src/WTangent.Components 生成器），打包项目收拢单包 0.0.2 ([07bfcad](https://github.com/WTangent-Org/WTangent.Components/commit/07bfcad5502042d72152246ff5ea9732e7abc624))
* 组件共享包合并——源生成器 + 运行时契约单包 WTangent.Components 0.4.0 ([6881a12](https://github.com/WTangent-Org/WTangent.Components/commit/6881a12f7abe309ae91fa782b3b71a4717319ded))


### Bug Fixes

* initial-version 0.1.0→0.0.1（首个版本从 0.0.1 起步） ([23d7def](https://github.com/WTangent-Org/WTangent.Components/commit/23d7def9677e84c1e6cfbbf8034ae1f51e61c8f9))
* release.yml 重复头部（startup_failure）；版本 0.0.1 起步 ([733e977](https://github.com/WTangent-Org/WTangent.Components/commit/733e977aceb9099c0860bb7be523f72601d0f4e6))
* 特性类移入契约项目（lib 编译引用），生成器只留 analyzers——组件 [AgentComponent] 可解析 ([4d8708a](https://github.com/WTangent-Org/WTangent.Components/commit/4d8708a8760d3b6744c11904267eaa3b7210034f))
