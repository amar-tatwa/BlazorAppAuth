using BlazorAppAuth.Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
services.AddHttpClient(nameof(BaseAuthenticationStateProvider), client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); // Use the base address from host environment
});

services.AddAuthorizationCore();
services.AddCascadingAuthenticationState();

// Register the custom authentication state provider
services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
await builder.Build().RunAsync();
