using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Repositories.Common;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected static ListQueryOptions ToOptions(CollectionQueryRequest query) => query.ToListQueryOptions();

    protected static QuerySpecification ToSpec(CollectionQueryRequest query) => ToOptions(query).ToSpecification();

    protected bool AcceptsXml()
    {
        var accept = Request.GetTypedHeaders().Accept;
        return accept.Count > 0 && accept.Any(mediaType =>
            mediaType.MediaType.HasValue &&
            string.Equals(mediaType.MediaType.Value, "application/xml", StringComparison.OrdinalIgnoreCase));
    }

    protected IActionResult OkResponse<T>(T data, string message = "Request processed successfully") =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(string routeName, object routeValues, T data) =>
        StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Ok(data, "Resource created successfully"));

    protected IActionResult BadRequestResponse(string message, object? errors = null) =>
        BadRequest(ApiResponse<object>.Fail(message, errors));

    protected IActionResult OkPagedResponse<TBusiness, TItem>(
        PagedBusinessResult<TBusiness> result,
        Func<TBusiness, TItem> mapper,
        CollectionQueryRequest query)
    {
        var options = ToOptions(query);
        var mappedItems = result.Items.Select(mapper).ToList();
        var pagination = new PaginationMetadata
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        if (AcceptsXml())
        {
            var typedData = new PagedListResponse<TItem>
            {
                Items = mappedItems,
                Pagination = pagination
            };
            return Ok(ApiResponse<PagedListResponse<TItem>>.Ok(typedData));
        }

        var items = mappedItems.Cast<object>().ToList();
        var data = new PagedListResponse<object>
        {
            Items = FieldSelectionHelper.ApplyFieldsToList(items, options.GetFields()),
            Pagination = pagination
        };

        return Ok(ApiResponse<PagedListResponse<object>>.Ok(data));
    }

    protected IActionResult OkDetailResponse<TItem>(TItem response, CollectionQueryRequest query)
    {
        if (AcceptsXml())
            return Ok(ApiResponse<TItem>.Ok(response));

        return Ok(ApiResponse<object>.Ok(FieldSelectionHelper.ApplyFields(response, ToOptions(query).GetFields())));
    }
}
