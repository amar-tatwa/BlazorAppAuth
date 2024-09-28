using BlazorAppAuth.Auth;
using BlazorAppAuth.Client.Auth;
using BlazorAppAuth.Client.Pages;
using BlazorAppAuth.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRadzenComponents();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
var services = builder.Services;
// Read base address from configuration
var baseAddress = builder.Configuration["ApiSettings:BaseAddress"];
services.AddHttpClient<BaseAuthenticationStateProvider>(client => { client.BaseAddress = new Uri(baseAddress); });

services.AddControllers();

services.AddCascadingAuthenticationState();
services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        options =>
        {
            options.LoginPath = "/login-2";
            options.AccessDeniedPath = "/access-denied";
        });

services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAppAuth.Client._Imports).Assembly);
app.MapControllers();
app.Run();
