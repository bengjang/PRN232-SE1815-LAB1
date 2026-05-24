using Microsoft.AspNetCore.Mvc;

namespace PRN232.LMS.API.Models.Requests;

public class CollectionQueryRequest
{
    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "size")]
    public int Size { get; set; } = 10;

    [FromQuery(Name = "fields")]
    public string? Fields { get; set; }

    [FromQuery(Name = "expand")]
    public string? Expand { get; set; }
}
