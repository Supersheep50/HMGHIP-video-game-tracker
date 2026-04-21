using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlayedGames.Components;
using PlayedGames.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddHttpClient<RawgService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<FirebaseService>();

await builder.Build().RunAsync();
