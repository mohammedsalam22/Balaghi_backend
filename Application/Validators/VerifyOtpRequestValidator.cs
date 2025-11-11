using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email required")
                .EmailAddress().WithMessage("Email format is incorrect");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("code required")
                .Length(6).WithMessage("The verification code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("The verification code must be numbers only");
        }
    }
}
