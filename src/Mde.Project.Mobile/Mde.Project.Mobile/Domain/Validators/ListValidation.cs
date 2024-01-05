﻿using FluentValidation;
using Mde.Project.Mobile.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mde.Project.Mobile.Domain.Validators
{
    public class ListValidation:AbstractValidator<ListModel>
    {
        public ListValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Please provide a name!");
        }
    }
}
