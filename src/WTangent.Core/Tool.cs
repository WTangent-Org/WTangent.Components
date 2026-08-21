using System.Text.Json;

namespace WTangent.Core;

/// <summary>LLM 工具契约：serve 内置工具与工具组件（Type=="tool"）共用。
/// 工具组件 dll 引用本包实现 ITool，serve 启动时扫描已装组件目录加载并合并进工具列表。</summary>
public interface ITool
{
    /// <summary>工具名（LLM 调用时使用）</summary>
    string Name { get; }

    /// <summary>OpenAI function calling 定义</summary>
    object Definition { get; }

    /// <summary>执行工具，arguments 为 JSON 字符串，返回文本结果</summary>
    Task<string> RunAsync(string arguments, CancellationToken ct = default);
}

/// <summary>从 arguments JSON 读取字符串参数（工具实现辅助）</summary>
public static class ToolArgs
{
    public static string GetString(string arguments, string prop)
    {
        try
        {
            using var doc = JsonDocument.Parse(arguments);
            if (doc.RootElement.TryGetProperty(prop, out var p))
                return p.GetString() ?? "";
        }
        catch { }
        return "";
    }
}
