using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace ShopManager.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleErrors(List<Error> errors)
        {
            return errors.First().Type switch
            {
                //ErrorType.NotFound => NotFound(errors.First().Description),
                ErrorType.NotFound => NotFound(new ProblemDetails
                {
                    Status = NotFound().StatusCode,
                    Detail = errors.First().Description,
                    Title = errors.First().Code
                }),
                ErrorType.Validation => ValidationProblem(new ValidationProblemDetails
                {
                    Status = NotFound().StatusCode,
                    Detail = errors.First().Description,
                    Title = errors.First().Code
                }),
                ErrorType.Conflict => Conflict(new ProblemDetails
                {
                    Status = NotFound().StatusCode,
                    Detail = errors.First().Description,
                    Title = errors.First().Code
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, errors.First().Description)
            };
        }
    }
}
