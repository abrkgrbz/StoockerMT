using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tenant name is required")
                .Length(3, 100).WithMessage("Tenant name must be between 3 and 100 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Tenant code is required")
                .Matches(@"^[A-Z][A-Z0-9]{2,9}$").WithMessage("Tenant code must start with letter and be 3-10 alphanumeric characters");

            //RuleFor(x => x.AdminEmail)
            //    .NotEmpty().WithMessage("Admin email is required")
            //    .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.AdminPassword)
                .NotEmpty().WithMessage("Admin password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.AdminFirstName)
                .NotEmpty().WithMessage("Admin first name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters");

            RuleFor(x => x.AdminLastName)
                .NotEmpty().WithMessage("Admin last name is required")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters");
        }
    }
}
