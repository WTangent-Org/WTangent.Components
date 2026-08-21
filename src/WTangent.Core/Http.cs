namespace WTangent.Core;

/// <summary>统一 HttpClient（组件仓长期共用，避免到处 new 导致连接池碎片/端口耗尽）：
/// 默认 15s 超时 + 系统代理。需要长超时的场景用 `using var client = Http.New(timeout)`。</summary>
public static class Http
{
    /// <summary>默认共享实例（短请求：索引/版本检查/install 触发）</summary>
    public static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromSeconds(15),
    };

    /// <summary>按需新实例（下载大文件等长超时场景；用后 Dispose）</summary>
    public static HttpClient New(TimeSpan timeout)
    {
        var client = new HttpClient { Timeout = timeout };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("wtagent");
        return client;
    }
}
