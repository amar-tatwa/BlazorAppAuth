using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using BlazorAppAuth.Client.Models;

namespace BlazorAppAuth.Client.Auth;

internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        bool tryTakeSuccess = state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo);
        if (!tryTakeSuccess || userInfo is null)
        {
            return;
        }

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, userInfo.user_id),
                new Claim(ClaimTypes.Name, userInfo.user_name),
                //new Claim(ClaimTypes.Email, userInfo.Email)
                ];

        //if (!string.IsNullOrEmpty(userInfo.Roles))
        //{
        //    string[] roles = userInfo.Roles.Split(',');

        //    foreach (var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //}

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}