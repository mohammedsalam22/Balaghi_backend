using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Application.DTOs;
namespace Application.Validators
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(" Name required")
                .MaximumLength(100).WithMessage("The name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email required")
                .EmailAddress().WithMessage("Email Not found");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password required")
                .MinimumLength(8).WithMessage("The password must be at least 8 characters long");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("The password and confirmation do not match");
        }
    }
}