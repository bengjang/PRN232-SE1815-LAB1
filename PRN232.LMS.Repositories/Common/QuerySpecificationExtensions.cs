namespace PRN232.LMS.Repositories.Common;

public static class QuerySpecificationExtensions
{
    /// <summary>List API: expand is opt-in.</summary>
    public static bool ShouldExpandForList(this QuerySpecification spec, string name) =>
        spec.ShouldExpand(name);

    /// <summary>Get by id: no expand param = include all relations (lab §4); otherwise opt-in.</summary>
    public static bool ShouldExpandForDetail(this QuerySpecification spec, string name) =>
        spec.Expansions.Count == 0 || spec.ShouldExpand(name);
}
