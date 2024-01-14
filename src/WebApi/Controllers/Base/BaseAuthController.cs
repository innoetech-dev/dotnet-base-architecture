using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Application.Interfaces.Token;

namespace WebApi.Controllers.Base
{
    public class BaseAuthController : Controller
    {
        private readonly ITokenServices _tokenServices;
        private readonly IConfiguration _configuration;

        public BaseAuthController(ITokenServices tokenServices,
            IConfiguration configuration)
        {
            _tokenServices = tokenServices;
            _configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(a => a.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .ToList();
                context.Result = new BadRequestObjectResult(errors);
            }

            var headers = context.HttpContext.Request.Headers;

            if (headers.ContainsKey("Authorization"))
            {
                bool isTokenValidated = false;
                var accessToken = headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    isTokenValidated = _tokenServices
                    .ValidateTokenWithExpiryTime("fdsfksdfksdgkfsdfsfgssgskgsglw005694sfmsg0750wesd040460232",
                    "http://localhost", "http://localhost", accessToken);
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult("Invalid Token");
                }
                if (!isTokenValidated)
                {
                    context.Result = new UnauthorizedObjectResult("Invalid Token");
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
