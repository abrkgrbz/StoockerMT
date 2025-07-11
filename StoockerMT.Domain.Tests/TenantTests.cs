using FluentAssertions;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using Xunit;

namespace StoockerMT.Domain.Tests.Entities.MasterDb
{
    public class TenantTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateTenant()
        {
            // Arrange
            var name = "Test Company";
            var code = new TenantCode("TST001");
            var createdBy = "TestUser";
            var description = "Test Description";

            // Act
            var tenant = new Tenant(name, code, createdBy, description);

            // Assert
            tenant.Should().NotBeNull();
            tenant.Name.Should().Be(name);
            tenant.Code.Should().Be(code);
            tenant.CreatedBy.Should().Be(createdBy);
            tenant.Description.Should().Be(description);
            tenant.Status.Should().Be(TenantStatus.Pending);
            tenant.IsActive().Should().BeFalse();
            tenant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var code = new TenantCode("TST001");

            // Act & Assert
            var act = () => new Tenant(invalidName, code, "TestUser");
            act.Should().Throw<ArgumentException>()
                .WithMessage("*name*");
        }

        [Fact]
        public void Activate_WhenPending_ShouldActivateTenant()
        {
            // Arrange
            var tenant = CreateTestTenant();
            var dbInfo = CreateDatabaseInfo();
            tenant.SetDatabaseInfo(dbInfo);
            // Act
            tenant.Activate("SystemAdmin");

            // Assert
            tenant.Status.Should().Be(TenantStatus.Active);
            tenant.IsActive().Should().BeTrue();
            tenant.ActivatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tenant.CreatedBy.Should().Be("SystemAdmin");
        }

        [Fact]
        public void Deactivate_WhenActive_ShouldDeactivateTenant()
        {
            // Arrange
            var tenant = CreateTestTenant();
            tenant.SetDatabaseInfo(CreateDatabaseInfo());
            tenant.Activate("SystemAdmin");
            var reason = "Non-payment";

            // Act
            tenant.Deactivate(reason, "SystemAdmin");

            // Assert
            tenant.Status.Should().Be(TenantStatus.Inactive);
            tenant.IsActive().Should().BeFalse();
            tenant.DeactivatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tenant.UpdatedBy.Should().Be("SystemAdmin");
            tenant.DeactivationReason.Should().Be(reason);
        }

        [Fact]
        public void Suspend_WhenActive_ShouldSuspendTenant()
        {
            // Arrange
            var tenant = CreateTestTenant();
            tenant.SetDatabaseInfo(CreateDatabaseInfo());
            tenant.Activate("SystemAdmin");
            var reason = "Policy violation";

            // Act
            tenant.Suspend(reason, "SystemAdmin");

            // Assert
            tenant.Status.Should().Be(TenantStatus.Suspended);
            tenant.IsActive().Should().BeFalse();
            tenant.DeactivatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tenant.UpdatedBy.Should().Be("SystemAdmin");
            tenant.DeactivationReason.Should().Be(reason);
        }

        [Fact]
        public void UpdateDatabaseInfo_WithValidInfo_ShouldUpdateDatabaseInfo()
        {
            // Arrange
            var tenant = CreateTestTenant();
             tenant.SetDatabaseInfo(CreateDatabaseInfo());


            tenant.DatabaseInfo.Should().Be(CreateDatabaseInfo());
            tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tenant.UpdatedBy.Should().Be("SystemAdmin");
        }

        //[Fact]
        //public void CanAddModule_WhenActiveAndBelowMaxModules_ShouldReturnTrue()
        //{
        //    // Arrange
        //    var tenant = CreateTestTenant();
        //    tenant.Activate("SystemAdmin");
        //    tenant.UpdateLimits(20,0,5, "SystemAdmin");

        //    // Act
        //    var result = tenant.CanAddModule();

        //    // Assert
        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void CanAddUser_WhenActiveAndBelowMaxUsers_ShouldReturnTrue()
        //{
        //    // Arrange
        //    var tenant = CreateTestTenant();
        //    tenant.Activate("SystemAdmin");
        //    tenant.UpdateSettings(tenant.Settings with { MaxUsers = 10 }, "SystemAdmin");

        //    // Act
        //    var result = tenant.CanAddUser();

        //    // Assert
        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void IsTrialExpired_WhenTrialPeriodPassed_ShouldReturnTrue()
        //{
        //    // Arrange
        //    var tenant = CreateTestTenant();
        //    tenant.UpdateSettings(tenant.Settings with { TrialDays = 1 }, "SystemAdmin");

        //    // Simulate tenant created 2 days ago
        //    var createdAtProperty = typeof(Tenant).GetProperty("CreatedAt");
        //    createdAtProperty!.SetValue(tenant, DateTime.UtcNow.AddDays(-2));

        //    // Act
        //    var result = tenant.IsTrialExpired();

        //    // Assert
        //    result.Should().BeTrue();
        //}

        private static Tenant CreateTestTenant()
        {
            return new Tenant(
                name: "Test Company",
                code: new TenantCode("TST001"),
                createdBy: "SystemAdmin",
                description: "Test Description"
            );
        }

        private static DatabaseInfo CreateDatabaseInfo()
        {
           return DatabaseInfo.Create(
                databaseName: "TestDB",
                server: "localhost",
                username: "sa",
                encryptedPassword: "encrypted123",
                "Server=localhost;Database=TestDB;User Id=sa;Password=encrypted123;Encrypt=True;TrustServerCertificate=True;"
            ); 

        }
    }
}