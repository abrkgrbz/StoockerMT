using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class ModuleFeatureConfiguration : IEntityTypeConfiguration<ModuleFeature>
    {
        public void Configure(EntityTypeBuilder<ModuleFeature> builder)
        {
            builder.ToTable("ModuleFeatures");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(f => new { f.ModuleId, f.Code })
                .IsUnique()
                .HasDatabaseName("IX_ModuleFeatures_ModuleId_Code");

            builder.Property(f => f.Description)
                .HasMaxLength(500);

            builder.Property(f => f.IsEnabled)
                .HasDefaultValue(true);

            builder.Property(f => f.Configuration)
                .HasColumnType("nvarchar(max)");

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(f => f.Module)
                .WithMany(m => m.Features)
                .HasForeignKey(f => f.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
