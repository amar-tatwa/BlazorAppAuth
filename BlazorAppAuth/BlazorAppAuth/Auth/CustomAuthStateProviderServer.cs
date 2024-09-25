using BlazorAppAuth.Client.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorAppAuth.Auth;

public class CustomAuthStateProviderServer : BaseAuthenticationStateProvider
{
    public CustomAuthStateProviderServer(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory)
    {
    }

    protected override string GetAuthenticationType()
    {
        //return "cookie"; // Server-side authentication type
        return CookieAuthenticationDefaults.AuthenticationScheme;
    }
}
