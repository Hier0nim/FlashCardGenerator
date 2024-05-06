using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlashCardGenerator.Components;
using FlashCardGenerator.Components.Account;
using FlashCardGenerator.Data;
using FlashCardGenerator.Options;
using FlashCardGenerator.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using MudBlazor.Services;

var applicationBuilder = WebApplication.CreateBuilder(args);

ConfigureServices(applicationBuilder);

var application = applicationBuilder.Build();

ConfigurePipeline(application);

application.Run();
return;

void ConfigureServices(WebApplicationBuilder builder)
{
    // Basic services
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddMudServices();

    // Authentication and identity
    ConfigureIdentityServices(builder);

    // Database services
    ConfigureDatabaseServices(builder);

    // Custom application services
    ConfigureApplicationServices(builder);

    // Environment-specific services
    ConfigureEnvironmentSpecificServices(builder);
}

void ConfigureIdentityServices(WebApplicationBuilder builder)
{
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

    builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
}

void ConfigureDatabaseServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                          ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                        options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

void ConfigureApplicationServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IEmailSender, AzureCommunicationServiceEmailClient>();
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();
}

void ConfigureEnvironmentSpecificServices(WebApplicationBuilder builder)
{
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAi"));
        builder.Services.Configure<EMailOptions>(builder.Configuration.GetSection("EMail"));
        builder.Services.Configure<CommunicationServiceOptions>(builder.Configuration.GetSection("CommunicationService"));
    }
    else
    {
        var keyVaultConfig = builder.Configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultOptions>();
        var client = new SecretClient(new Uri(keyVaultConfig!.Uri), new DefaultAzureCredential());

        ConfigureSecrets(client, builder);
    }
}

void ConfigureSecrets(SecretClient client, WebApplicationBuilder builder)
{
    var openAiApiKeySecret = client.GetSecret("OpenAi--ApiKey");
    builder.Services.Configure<OpenAiOptions>(options => {
        options.ApiKey = openAiApiKeySecret.Value.Value;
    });

    var eMailSecret = client.GetSecret("EMail--DomainName");
    builder.Services.Configure<EMailOptions>(options => {
        options.DomainName = eMailSecret.Value.Value;
    });

    var communicationServiceSecret = client.GetSecret("CommunicationService--ConnectionString");
    builder.Services.Configure<CommunicationServiceOptions>(options => {
        options.ConnectionString = communicationServiceSecret.Value.Value;
    });
}

void ConfigurePipeline(WebApplication app)
{
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

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    app.MapAdditionalIdentityEndpoints();
}
