namespace WTangent.Core;

/// <summary>全局日志门面：组件任意位置 Log.Info(...)，免 Entry.App.Logger 长链。
/// 宿主启动早期 Init 注入实现（空壳 BuildApp）；单 ALC 下 Core 永远只有宿主一份，静态唯一。
/// 未 Init 时退化到 Console（组件独立测试场景），日志不丢、不抛异常。</summary>
public static class Log
{
    private static ILogger? _logger;

    /// <summary>宿主注入日志实现（进程启动早期调用一次；重复调用覆盖，测试可用）</summary>
    public static void Init(ILogger logger) => _logger = logger;

    public static void Debug(string msg) => Impl.Debug(msg);
    public static void Info(string msg) => Impl.Info(msg);
    public static void Warn(string msg) => Impl.Warn(msg);
    public static void Error(string msg, Exception? ex = null) => Impl.Error(msg, ex);

    private static ILogger Impl => _logger ?? ConsoleLogger.Instance;

    /// <summary>未 Init 时的兜底实现：控制台（级别前缀，与 HostLogger 同格式）</summary>
    private sealed class ConsoleLogger : ILogger
    {
        public static readonly ConsoleLogger Instance = new();

        public void Debug(string msg) => Write("DEBUG", msg);
        public void Info(string msg) => Write("INFO", msg);
        public void Warn(string msg) => Write("WARN", msg);
        public void Error(string msg, Exception? ex = null) => Write("ERROR", ex is null ? msg : $"{msg}\n{ex}");

        private static void Write(string level, string msg) =>
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {msg}");
    }
}
