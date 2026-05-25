using PRN232.LMS.Repositories.Common;

namespace PRN232.LMS.Services.BusinessModels;

public class ListQueryOptions
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Fields { get; set; }
    public string? Expand { get; set; }
    public int? CourseId { get; set; }

    public QuerySpecification ToSpecification()
    {
        var spec = new QuerySpecification
        {
            Search = Search,
            Sort = Sort,
            Page = Page,
            Size = Size,
            CourseId = CourseId
        };

        if (!string.IsNullOrWhiteSpace(Expand))
        {
            foreach (var part in Expand.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                spec.Expansions.Add(part);
        }

        spec.Normalize();
        return spec;
    }

    public IReadOnlyList<string> GetFields()
    {
        if (string.IsNullOrWhiteSpace(Fields))
            return [];

        return Fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(f => f.Trim())
            .ToList();
    }
}

