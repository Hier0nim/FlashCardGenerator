using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlashCardGenerator.Components;
using FlashCardGenerator.Components.Account;
using FlashCardGenerator.Data;
using FlashCardGenerator.Options;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
  })
  .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                      options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddSignInManager()
  .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

string openAiApiKey = string.Empty;
if (builder.Environment.IsDevelopment())
{
  // If in development, configure OpenAiOptions using the user secrets.
  builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAi"));
}
else
{
  // If in production, manually retrieve the OpenAI API key from Azure Key Vault.
  const string kvUri = "https://kv-flashcardgenerator.vault.azure.net/";
  var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
  var secret = client.GetSecret("OpenAi--ApiKey");
  var openAiOptions = new OpenAiOptions{
    ApiKey = secret.Value.Value
  };
  openAiApiKey = secret.HasValue ? secret.Value.Value : throw new InvalidOperationException("OpenAI API key not found in Azure Key Vault.");

  builder.Services.AddSingleton(openAiOptions);
}

var app = builder.Build();
app.MapGet("/ApiKey", () => openAiApiKey);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();