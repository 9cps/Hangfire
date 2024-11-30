using Hangfire.Models.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Hangfire.Api.Controllers;

[ApiController]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger) => _logger = logger;

    [HttpGet("login")]
    public IActionResult Login() => View(new LoginViewModel()); // Returns the Login view

    [HttpGet("")]
    public IActionResult BackToSite() => Redirect("/hangfire");

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Simulate authentication (replace with actual logic)
        if (model.Username == "admin" && model.Password == "P@ssw0rd")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuthTDR");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // ถ้าต้องการให้คุกกี้อยู่ตลอด
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // ตั้งเวลา expiration
            };
            await HttpContext.SignInAsync("MyCookieAuthTDR", new ClaimsPrincipal(claimsIdentity), authProperties);

            return Ok(); // Return success
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return BadRequest(ModelState); // Return errors
    }
}
