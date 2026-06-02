using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Auth.Queries.GetProfile;
using BookLAB.Application.Features.TabletAccounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabletAccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TabletAccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<TabletAccountsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TabletAccountsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TabletAccountsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TabletAccountsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TabletAccountsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("login-tablet")]
        public async Task<IActionResult> LoginTablet([FromForm] LoginTabletQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                if (result.Data.Token != null)
                    HttpContext.Response.Cookies.Append("accessToken", result.Data.Token,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            HttpOnly = true,
                            IsEssential = true,
                            Secure = true,
                            SameSite = SameSiteMode.None
                        });
                return Redirect(result.Data.ReturnUrl);
            }

            return Redirect(result.Data.ReturnUrl);
        }
    }
}
