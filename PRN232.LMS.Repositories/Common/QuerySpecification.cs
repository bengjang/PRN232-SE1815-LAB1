namespace PRN232.LMS.Repositories.Common;

public class QuerySpecification
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public HashSet<string> Expansions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Normalize()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 10;
        if (Size > 100) Size = 100;
    }

    public bool ShouldExpand(string name) => Expansions.Contains(name);
}

