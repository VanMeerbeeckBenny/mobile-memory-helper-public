using FluentValidation;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mde.Project.Mobile.Domain.Validators
{
    public class LoginValidation : AbstractValidator<LoginModel>
    {
        public LoginValidation()
        {
            RuleFor(registration => registration.Email)
              .NotEmpty()
                  .WithMessage("E-mail cannot be empty")
              .EmailAddress()
                  .WithMessage("Please enter a valid e-mail address");


            RuleFor(registration => registration.Password)
                .NotEmpty()
                    .WithMessage("Password can not be empty");     



        }
    }
}
