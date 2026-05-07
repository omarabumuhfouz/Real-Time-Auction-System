using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Domain.Users.Errors;

namespace AuthService.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithErrorCode(UserErrorCodes.IdRequired)
            .WithMessage("User ID is required.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithErrorCode(UserErrorCodes.CurrentPasswordRequired)
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithErrorCode(UserErrorCodes.NewPasswordRequired)
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithErrorCode(UserErrorCodes.NewPasswordLength)
            .WithMessage("New password must be at least 8 characters long.")
            .MaximumLength(100)
            .WithErrorCode(UserErrorCodes.NewPasswordLength)
            .Matches(@"[A-Z]").WithErrorCode(UserErrorCodes.NewPasswordFormat).WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithErrorCode(UserErrorCodes.NewPasswordFormat).WithMessage("Password must contain at least one number.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithErrorCode(UserErrorCodes.ConfirmPasswordRequired)
            .Equal(x => x.NewPassword)
            .WithErrorCode(UserErrorCodes.PasswordsDoNotMatch)
            .WithMessage("The new password and confirmation password do not match.");
    }
}