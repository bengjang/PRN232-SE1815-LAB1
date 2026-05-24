using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.Mappings;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkResponse<T>(T data, string message = "Request processed successfully") =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(string routeName, object routeValues, T data) =>
        StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Ok(data, "Resource created successfully"));

    protected IActionResult BadRequestResponse(string message, object? errors = null) =>
        BadRequest(ApiResponse<object>.Fail(message, errors));

    protected IActionResult OkPagedResponse<TResponse>(
        PagedBusinessResult<TResponse> result,
        Func<TResponse, object> mapper,
        IReadOnlyList<string> fields)
    {
        var items = result.Items.Select(mapper).ToList();
        var data = new PagedListResponse<object>
        {
            Items = FieldSelectionHelper.ApplyFieldsToList(items, fields),
            Pagination = new PaginationMetadata
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            }
        };

        return Ok(ApiResponse<PagedListResponse<object>>.Ok(data));
    }

    protected IActionResult OkSingleResponse<TResponse>(TResponse response, IReadOnlyList<string> fields) =>
        Ok(ApiResponse<object>.Ok(FieldSelectionHelper.ApplyFields(response, fields)));
}
