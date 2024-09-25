using BlazorAppAuth.Client.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorAppAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(TokenRequest tokenRequest)
    {
        var user= TempMaster.GetUsers().Where(x=>x.user_name==tokenRequest.user_name).FirstOrDefault();
        if (user!=null && user.password == tokenRequest.password)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.user_name) };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            var cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1), Secure = true };
            Response.Cookies.Append("APITest", "Test value", cookieOptions);

            //server-side redirect (Redirect method) inherently causes a full page load.
            return Redirect("/");
        }

        return Unauthorized();
    }
    [HttpGet]
    public IActionResult GetUser()
    {
        var user = new User
        (
            user_name : User.Identity?.Name
            //IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            //Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
        );

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

}
