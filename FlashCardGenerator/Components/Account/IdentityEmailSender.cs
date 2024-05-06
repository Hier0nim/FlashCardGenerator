using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using FlashCardGenerator.Data;

namespace FlashCardGenerator.Components.Account;

/// <summary>
/// Handles sending various types of emails related to account management such as confirmation links and password resets.
/// </summary>
internal sealed class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailSender _emailSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityEmailSender"/> class.
    /// </summary>
    /// <param name="emailSender">The core email sender service used for dispatching emails.</param>
    public IdentityEmailSender(IEmailSender emailSender)
    {
        this._emailSender = emailSender;
    }

    /// <summary>
    /// Sends an email with a link to confirm the user's account.
    /// </summary>
    /// <param name="user">The user to whom the confirmation link will be sent.</param>
    /// <param name="email">The email address to which the link will be sent.</param>
    /// <param name="confirmationLink">The URL of the confirmation link.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    /// <summary>
    /// Sends an email with a link to reset the user's password.
    /// </summary>
    /// <param name="user">The user to whom the reset link will be sent.</param>
    /// <param name="email">The email address to which the link will be sent.</param>
    /// <param name="resetLink">The URL of the reset link.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    /// <summary>
    /// Sends an email containing a code to reset the user's password.
    /// </summary>
    /// <param name="user">The user to whom the reset code will be sent.</param>
    /// <param name="email">The email address to which the code will be sent.</param>
    /// <param name="resetCode">The password reset code.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
