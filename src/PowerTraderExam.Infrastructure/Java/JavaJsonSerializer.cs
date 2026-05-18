using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using PowerTraderExam.Application.DTOs.Java;

namespace PowerTraderExam.Infrastructure.Java;

public static class JavaJsonSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    public static string SerializeGradeItems(IReadOnlyList<JavaGradeCallbackItemDto> items) =>
        JsonSerializer.Serialize(items, Options);
}
