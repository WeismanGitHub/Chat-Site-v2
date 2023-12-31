﻿namespace API.Endpoints.Account.Signup;

public sealed class Request {
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

internal sealed class Validator : Validator<Request> {
    public Validator() {
        RuleFor(account => account.DisplayName)
            .NotEmpty().WithMessage("DisplayName is required.")
            .MinimumLength(1).WithMessage("DisplayName cannot be empty.")
            .MaximumLength(User.MaxNameLength).WithMessage($"DisplayName cannot be longer than {User.MaxNameLength} characters.");

        RuleFor(account => account.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The format of your email address is invalid.");

        RuleFor(account => account.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(password => password.IsAValidPassword()).WithMessage("Password is invalid.");
    }
}
