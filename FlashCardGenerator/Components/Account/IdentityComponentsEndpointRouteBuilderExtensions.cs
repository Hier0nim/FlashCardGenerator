using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using FlashCardGenerator.Components.Account.Pages;
using FlashCardGenerator.Components.Account.Pages.Manage;
using FlashCardGenerator.Data;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
  {
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static void MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
      ArgumentNullException.ThrowIfNull(endpoints);

      var accountGroup = endpoints.MapGroup("/Account");

      accountGroup.MapPost("/Logout", async (
                             ClaimsPrincipal user,
                             SignInManager<ApplicationUser> signInManager,
                             [FromForm] string returnUrl) => {
                             await signInManager.SignOutAsync();
                             return TypedResults.LocalRedirect($"~/{returnUrl}");
                           });

      var manageGroup = accountGroup.MapGroup("/Manage")
        .RequireAuthorization();

      var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
      var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

      manageGroup.MapPost("/DownloadPersonalData", async (
                            HttpContext context,
                            [FromServices] UserManager<ApplicationUser> userManager,
                            [FromServices] AuthenticationStateProvider authenticationStateProvider) => {
                            var user = await userManager.GetUserAsync(context.User);
                            if (user is null)
                            {
                              return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                            }

                            var userId = await userManager.GetUserIdAsync(user);
                            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                            // Only include personal data for download
                            var personalData = new Dictionary<string, string>();
                            var personalDataProps = typeof(ApplicationUser).GetProperties()
                              .Where(
                              prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                            foreach (var p in personalDataProps)
                            {
                              personalData.Add(p.Name, p.GetValue(user)
                                                 ?.ToString() ?? "null");
                            }

                            var logins = await userManager.GetLoginsAsync(user);
                            foreach (var l in logins)
                            {
                              personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                            }

                            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
                          });
    }
  }