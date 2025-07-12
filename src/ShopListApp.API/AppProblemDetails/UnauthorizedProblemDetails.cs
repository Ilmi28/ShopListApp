using Microsoft.AspNetCore.Mvc;

namespace ShopListApp.API.AppProblemDetails;

public class UnathorizedProblemDetails : ProblemDetails
{
    public UnauthorizedProblemDetails(string? detail)
    {
        Title = "Unauthorized";
        Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2";
        Status = StatusCodes.Status401Unauthorized;
        Detail = detail;
    }
}