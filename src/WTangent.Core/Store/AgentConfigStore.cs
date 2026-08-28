using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace WTangent.Core;

/// <summary>单个提供商：BaseUrl + 模型（API Key 单独加密文件存储，不入 json）</summary>
public sealed record ProviderEntry
{
    public string Name { get; init; } = "";
    public string BaseUrl { get; init; } = "";
    public string Model { get; init; } = "";
    public string Variants { get; init; } = "Default";
    [JsonIgnore]
    public string ApiKey { get; init; } = "";
}

/// <summary>全局配置：多提供商，Active 指定当前使用的提供商</summary>
public sealed record AgentConfig
{
    public string Active { get; init; } = "deepseek";
    public List<ProviderEntry> Providers { get; init; } = [];
    /// <summary>收到 git push 后自动触发 agent 简单优化（默认关，省 token；WUI 设置里可开）</summary>
    public bool AutoOptimize { get; init; }
}

/// <summary>Agent 配置存取：公开部分（提供商清单/激活项/AutoOptimize）走 <see cref="Config"/> 门面
/// （config.json 的 "agent" 键，变更发 config.changed 事件）；API Key 永不入 json，
/// 单独 DPAPI 加密存 %APPDATA%\agent\apikey.{name}（原各仓 ConfigStore 并入，行为未改；
/// 旧版顶层 AgentConfig 格式自动迁移到 "agent" 键）</summary>
public static class AgentConfigStore
{
    private const string Key = "agent";

    private static string Dir => AgentPaths.DataDir;

    private static string KeyFile(string name) => Path.Combine(Dir, $"apikey.{Sanitize(name)}");
    private static string Sanitize(string name) =>
        string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));

    public static AgentConfig Load()
    {
        var cfg = Config.Get<AgentConfig>(Key) ?? MigrateLegacy();
        return cfg with { Providers = [.. cfg.Providers.Select(p => p with { ApiKey = ReadKey(p.Name) })] };
    }

    /// <summary>旧版迁移：ConfigStore 时代 config.json 顶层即 AgentConfig 结构，DefaultConfig 按 KV 读入后
    /// 顶层各字段成了独立键。检出旧结构 → 重组写入 "agent" 键并清掉顶层旧键（一次性，幂等）</summary>
    private static AgentConfig MigrateLegacy()
    {
        if (Config.Get<ProviderEntry[]>("Providers") is not { Length: > 0 } providers)
            return new AgentConfig();
        var cfg = new AgentConfig
        {
            Active = Config.Get<string>("Active") ?? "deepseek",
            AutoOptimize = Config.Get<bool>("AutoOptimize"),
            Providers = [.. providers],
        };
        Save(cfg);
        Config.Remove("Active");
        Config.Remove("Providers");
        Config.Remove("AutoOptimize");
        return cfg;
    }

    /// <summary>当前激活的提供商（含 API Key），无则返回 null</summary>
    public static ProviderEntry? LoadActive()
    {
        var cfg = Load();
        return cfg.Providers.FirstOrDefault(p => p.Name == cfg.Active) ?? cfg.Providers.FirstOrDefault();
    }

    public static void Save(AgentConfig cfg)
    {
        // ApiKey 不入 config.json（[JsonIgnore] 之外再显式剥一次，双保险）
        Config.Set(Key, cfg with { Providers = [.. cfg.Providers.Select(p => p with { ApiKey = "" })] });
        foreach (var p in cfg.Providers.Where(p => !string.IsNullOrEmpty(p.ApiKey)))
            WriteKey(p.Name, p.ApiKey);
    }

    private static string ReadKey(string name)
    {
        var file = KeyFile(name);
        if (!File.Exists(file)) return "";
        try
        {
            var data = File.ReadAllBytes(file);
            if (OperatingSystem.IsWindows())
                data = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return System.Text.Encoding.UTF8.GetString(data);
        }
        catch { return ""; }
    }

    private static void WriteKey(string name, string apiKey)
    {
        Directory.CreateDirectory(Dir);
        var data = System.Text.Encoding.UTF8.GetBytes(apiKey);
        if (OperatingSystem.IsWindows())
            data = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(KeyFile(name), data);
    }
}
