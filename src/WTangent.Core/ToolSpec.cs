using System.Text.Json;

namespace WTangent.Core;

/// <summary>工具 Definition 构造助手:消灭每个工具 30-50 行的匿名对象样板。
/// 一行一个参数,链式声明,最后 Build() 产出 OpenAI function calling 定义。</summary>
public sealed class ToolSpec(string name, string description)
{
    private sealed record Prop(string Name, string Type, string Desc, bool Required, object? Items);

    private readonly List<Prop> _props = [];

    public static ToolSpec Create(string name, string description) => new(name, description);

    public ToolSpec Param(string name, string type, string description)
    {
        _props.Add(new Prop(name, type, description, false, null));
        return this;
    }

    public ToolSpec Required(string name, string type, string description)
    {
        _props.Add(new Prop(name, type, description, true, null));
        return this;
    }

    public ToolSpec ArrayParam(string name, string itemType, string description)
    {
        _props.Add(new Prop(name, "array", description, false, new { type = itemType }));
        return this;
    }

    public object Build() => new
    {
        type = "function",
        function = new
        {
            name,
            description,
            parameters = new
            {
                type = "object",
                properties = _props.ToDictionary(
                    p => p.Name,
                    p => p.Items is null
                        ? (object)new { type = p.Type, description = p.Desc }
                        : new { type = p.Type, description = p.Desc, items = p.Items }),
                required = _props.Where(p => p.Required).Select(p => p.Name).ToArray(),
            },
        },
    };

    /// <summary>序列化为 JSON 文本(调试/日志用)。</summary>
    public override string ToString() => JsonSerializer.Serialize(Build());
}
