using FluentValidation;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Validators
{
    public class EmailValidation:AbstractValidator<EmailModel>
    {
        public EmailValidation()
        {
            RuleFor(registration => registration.Email)
               .NotEmpty()
                   .WithMessage("E-mail cannot be empty")
               .EmailAddress()
                   .WithMessage("Please enter a valid e-mail address");
        }
    }
}
