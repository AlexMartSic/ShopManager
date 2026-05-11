using ShopManager.Application.Exceptions;

namespace ShopManager.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IProblemDetailsService _problemDetailsService;

        public ErrorHandlingMiddleware(RequestDelegate next, IProblemDetailsService problemDetailsService)
        {
            _next = next;
            _problemDetailsService = problemDetailsService;
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                NotFoundException e => (StatusCodes.Status404NotFound, "Resource not found"),
                ValidationException e => (StatusCodes.Status400BadRequest, "Validation error"),
                ConflictException e => (StatusCodes.Status409Conflict, "Conflict"),
                ArgumentNullException e => (StatusCodes.Status400BadRequest, "Argument null error"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            context.Response.StatusCode = statusCode;

            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails =
                {
                    Title = title,
                    Detail = ex.Message,
                    Status = statusCode
                }
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
    }
}
