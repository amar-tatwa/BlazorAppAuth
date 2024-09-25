﻿using BlazorAppAuth.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace BlazorAppAuth.Auth;
internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        scopeFactory = serviceScopeFactory;
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Extract the ClaimsPrincipal from the authentication state
        var user = authenticationState.User;

        // Check if the user is authenticated
        if (user.Identity is { IsAuthenticated: false })
        {
            return false; // User is not authenticated, so the state is invalid
        }

        // Optionally, validate specific claims
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return false; // No user ID claim found, invalid state
        }

        // Get the user manager from a new scope to ensure it fetches fresh data
        //await using var scope = scopeFactory.CreateAsyncScope();
        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //return await ValidateSecurityStampAsync(userManager, authenticationState.User);
        // TODO check user state, possible check tokens
        return true;
    }

    //private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
    //{
    //    var user = await userManager.GetUserAsync(principal);
    //    if (user is null)
    //    {
    //        return false;
    //    }
    //    else if (!userManager.SupportsUserSecurityStamp)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
    //        var userStamp = await userManager.GetSecurityStampAsync(user);
    //        return principalStamp == userStamp;
    //    }
    //}

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
            var name = principal.FindFirst(options.ClaimsIdentity.UserNameClaimType)?.Value;
            IEnumerable<Claim> roles = principal.FindAll(options.ClaimsIdentity.RoleClaimType);

            string rolesStr = string.Join(",", roles.Select(item => item.Value));
            if (userId != null && email != null)
            {
                state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    user_id = userId,
                    user_name = name,
                    //Roles = rolesStr
                    //UserId = userId,
                    //Email = email,
                    //Name = name,
                    //Roles = rolesStr
                });
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}
