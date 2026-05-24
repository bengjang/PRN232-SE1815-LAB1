namespace PRN232.LMS.Repositories.Common;

public class PagedEntityResult<TEntity> where TEntity : class
{
    public List<TEntity> Items { get; set; } = [];
    public int TotalItems { get; set; }
}

