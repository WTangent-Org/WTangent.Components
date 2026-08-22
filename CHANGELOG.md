# Changelog

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
