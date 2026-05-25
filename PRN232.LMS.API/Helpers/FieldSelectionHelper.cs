using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN232.LMS.API.Helpers;

public static class FieldSelectionHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static object ApplyFields<T>(T source, IReadOnlyList<string> fields)
    {
        if (fields.Count == 0)
            return source!;

        var element = JsonSerializer.SerializeToElement(source, JsonOptions);
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in fields)
        {
            var property = element.EnumerateObject()
                .FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property.Value.ValueKind is JsonValueKind.Undefined)
                continue;

            result[property.Name] = JsonSerializer.Deserialize<object>(property.Value.GetRawText());
        }

        return result;
    }

    public static List<object> ApplyFieldsToList<T>(IEnumerable<T> sources, IReadOnlyList<string> fields) =>
        sources.Select(item => ApplyFields(item, fields)).ToList();
}
