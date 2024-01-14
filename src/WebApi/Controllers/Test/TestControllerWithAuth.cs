using Application.Interfaces.Token;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.Test
{
    [ApiController]
    [Route("controller")]
    public class TestControllerWithAuth : BaseAuthController
    {
        public TestControllerWithAuth(ITokenServices tokenServices, IConfiguration configuration) : base(tokenServices, configuration)
        {
        }

        [HttpPost]
        [Route("test")]
        public async Task<IActionResult> TestAsync()
        {
            return Ok();
        }
    }
}
