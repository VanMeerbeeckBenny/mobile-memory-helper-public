using FluentValidation;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Validators
{
    public class RegistrationValidator : AbstractValidator<RegistrationModel>
    {
        public RegistrationValidator()
        {
            RuleFor(registration => registration.Email)
                .NotEmpty()
                    .WithMessage("E-mail cannot be empty")
                .EmailAddress()
                    .WithMessage("Please enter a valid e-mail address");

            RuleFor(registration => registration.UserName)
                .NotEmpty()
                    .WithMessage("Username can not be empty")
                .MinimumLength(4)
                    .WithMessage("Username must be at least 4 charakter long")
                .MaximumLength(40)
                    .WithMessage("Username can not exceed 40 charakters");

            RuleFor(registration => registration.Password)
                  .Must(password =>
                  {
                      if (!string.IsNullOrWhiteSpace(password)) return char.IsUpper(password[0]);
                      else return false;
                  })
                .WithMessage("Password must start with a upper case")
                .NotEmpty()
                    .WithMessage("Password can not be empty")
                .MinimumLength(6)
                    .WithMessage("Password must be at least 6 charakter long")
                .MaximumLength(40)
                    .WithMessage("Password can not exceed 40 charakters");
              

            RuleFor(registration => registration.VerifyPassword)
                .NotEmpty()
                    .WithMessage("Password can not be empty")
                .Equal(registration => registration.Password)
                    .WithMessage("Verify password dosn't match password");           


        }
    }
}
