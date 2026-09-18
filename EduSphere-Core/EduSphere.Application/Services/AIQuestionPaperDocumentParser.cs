using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using EduSphere.Application.DTOs.AI;

namespace EduSphere.Application.Services;

public static partial class AIQuestionPaperDocumentParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static GeneratedQuestionPaperDocument ParseAndValidate(string content, decimal expectedMarks)
    {
        var json = ExtractJson(content);
        var document = JsonSerializer.Deserialize<GeneratedQuestionPaperDocument>(json, JsonOptions)
            ?? throw new InvalidOperationException("The AI provider returned an empty document.");

        if (string.IsNullOrWhiteSpace(document.Title) || document.Sections.Count == 0)
            throw new InvalidOperationException("The generated paper must contain a title and at least one section.");

        var questions = document.Sections.SelectMany(s => s.Questions).ToList();
        if (questions.Count == 0 || questions.Any(q => string.IsNullOrWhiteSpace(q.Text) || string.IsNullOrWhiteSpace(q.Answer) || q.Marks <= 0))
            throw new InvalidOperationException("Every generated question must contain text, an answer, and positive marks.");

        var actualMarks = questions.Sum(q => q.Marks);
        if (actualMarks != expectedMarks)
            throw new InvalidOperationException($"Generated marks total {actualMarks}, but {expectedMarks} was requested.");

        var duplicate = questions.GroupBy(q => Normalize(q.Text)).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null)
            throw new InvalidOperationException("The generated paper contains duplicate questions.");

        return document;
    }

    public static string SerializeStudentPaper(GeneratedQuestionPaperDocument document) =>
        JsonSerializer.Serialize(new
        {
            document.Title,
            Sections = document.Sections.Select(s => new
            {
                s.Code, s.Title, s.Instructions,
                Questions = s.Questions.Select(q => new { q.Text, q.Marks, q.QuestionType, q.Difficulty, q.BloomLevel, q.SyllabusUnit })
            })
        }, JsonOptions);

    public static string SerializeMarkingScheme(GeneratedQuestionPaperDocument document) =>
        JsonSerializer.Serialize(new
        {
            document.Title,
            Sections = document.Sections.Select(s => new
            {
                s.Code, s.Title,
                Questions = s.Questions.Select(q => new { q.Text, q.Answer, q.Marks, q.QuestionType, q.Difficulty, q.BloomLevel, q.SyllabusUnit })
            })
        }, JsonOptions);

    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    public static string RemovePotentialPii(string value)
    {
        value = EmailRegex().Replace(value, "[email removed]");
        value = PhoneRegex().Replace(value, "[phone removed]");
        return IdentityNumberRegex().Replace(value, "[identifier removed]");
    }

    private static string ExtractJson(string content)
    {
        var trimmed = content.Trim();
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var firstBreak = trimmed.IndexOf('\n');
            var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (firstBreak >= 0 && lastFence > firstBreak)
                trimmed = trimmed[(firstBreak + 1)..lastFence].Trim();
        }
        return trimmed;
    }

    private static string Normalize(string value) => string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    [GeneratedRegex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"(?<!\d)(?:\+?\d[\d\s-]{8,}\d)(?!\d)")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"(?<!\d)\d{12}(?!\d)")]
    private static partial Regex IdentityNumberRegex();
}
