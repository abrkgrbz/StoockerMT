using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Constants
{
    public static class Permissions
    {
        public static class Tenants
        {
            public const string View = "Permissions.Tenants.View";
            public const string Create = "Permissions.Tenants.Create";
            public const string Edit = "Permissions.Tenants.Edit";
            public const string Delete = "Permissions.Tenants.Delete";
        }

        public static class Customers
        {
            public const string View = "Permissions.Customers.View";
            public const string Create = "Permissions.Customers.Create";
            public const string Edit = "Permissions.Customers.Edit";
            public const string Delete = "Permissions.Customers.Delete";
            public const string ViewCreditInfo = "Permissions.Customers.ViewCreditInfo";
        }

        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
            public const string EditPricing = "Permissions.Products.EditPricing";
        }

        public static class Orders
        {
            public const string View = "Permissions.Orders.View";
            public const string Create = "Permissions.Orders.Create";
            public const string Edit = "Permissions.Orders.Edit";
            public const string Delete = "Permissions.Orders.Delete";
            public const string Cancel = "Permissions.Orders.Cancel";
            public const string Ship = "Permissions.Orders.Ship";
        }

        public static class Accounting
        {
            public const string ViewReports = "Permissions.Accounting.ViewReports";
            public const string CreateJournalEntry = "Permissions.Accounting.CreateJournalEntry";
            public const string PostJournalEntry = "Permissions.Accounting.PostJournalEntry";
            public const string ViewBalanceSheet = "Permissions.Accounting.ViewBalanceSheet";
        }
    }
}
