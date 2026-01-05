using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Base controller with helper methods for handling ServiceResult responses
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Converts a ServiceResult to an appropriate IActionResult based on status code
    /// </summary>
    protected IActionResult HandleResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
            return Ok(result.Data);

        return result.StatusCode switch
        {
            400 => BadRequest(new { error = result.ErrorMessage }),
            403 => Forbid(),
            404 => NotFound(new { error = result.ErrorMessage }),
            409 => Conflict(new { error = result.ErrorMessage }),
            _ => StatusCode(result.StatusCode ?? 500, new { error = result.ErrorMessage })
        };
    }
}

