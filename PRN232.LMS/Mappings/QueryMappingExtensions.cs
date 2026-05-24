using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Mappings;

public static class QueryMappingExtensions
{
    public static ListQueryOptions ToListQueryOptions(this CollectionQueryRequest request) =>
        new()
        {
            Search = request.Search,
            Sort = request.Sort,
            Page = request.Page,
            Size = request.Size,
            Fields = request.Fields,
            Expand = request.Expand
        };
}
