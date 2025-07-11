using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Application.Common.Models;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommand: IRequest<Result<CreateTenantResponseData>>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Email AdminEmail { get; set; } = new Email(string.Empty);
        public string AdminPassword { get; set; } = string.Empty;
        public string AdminFirstName { get; set; } = string.Empty;
        public string AdminLastName { get; set; } = string.Empty;
        public List<string> SelectedModules { get; set; } = new();
        public SubscriptionType SubscriptionType { get; set; } = SubscriptionType.Free;
        public Money SubscriptionPrice { get; set; } = new Money(0, "USD");
    }
}
