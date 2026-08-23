namespace WTangent.Core;

/// <summary>全局配置门面：组件任意位置 Config.Get/Set/Remove，免 Entry.App.Config 长链。
/// 宿主启动早期 Init 注入实现（空壳 BuildApp）；单 ALC 下 Core 永远只有宿主一份，静态唯一。
/// 未 Init 时退化为进程内内存字典（组件独立测试场景；不持久化、不发 config.changed 事件）。</summary>
public static class Config
{
    private static IConfig? _config;

    /// <summary>宿主注入配置实现（进程启动早期调用一次；重复调用覆盖，测试可用）</summary>
    public static void Init(IConfig config) => _config = config;

    public static T? Get<T>(string key) => Impl.Get<T>(key);
    public static void Set<T>(string key, T value) => Impl.Set(key, value);
    public static void Remove(string key) => Impl.Remove(key);

    private static IConfig Impl => _config ?? MemoryConfig.Instance;

    /// <summary>未 Init 时的兜底实现：进程内内存字典（与 HostConfig 同样的取值语义；不持久化、不发事件）</summary>
    private sealed class MemoryConfig : IConfig
    {
        public static readonly MemoryConfig Instance = new();
        private readonly Dictionary<string, object?> _data = new(StringComparer.OrdinalIgnoreCase);

        public T? Get<T>(string key)
        {
            lock (_data)
            {
                if (!_data.TryGetValue(key, out var v) || v is null) return default;
                try { return (T)Convert.ChangeType(v, typeof(T)); }
                catch { return System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(v)); }
            }
        }

        public void Set<T>(string key, T value)
        {
            lock (_data) _data[key] = value;
        }

        public void Remove(string key)
        {
            lock (_data) _data.Remove(key);
        }
    }
}
