using HardshipApp.Models;
using HardshipApp.Repositories.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardshipApp.Repositories.Configurations;

internal sealed class HardshipApplicationConfiguration : IEntityTypeConfiguration<HardshipApplication>
{
    public void Configure(EntityTypeBuilder<HardshipApplication> builder)
    {
        builder.ToTable("HardshipApplications", table =>
        {
            // A deficit is allowed; a negative income or expense figure is not.
            table.HasCheckConstraint("CK_HardshipApplications_MonthlyIncome", "\"MonthlyIncome\" >= 0");
            table.HasCheckConstraint("CK_HardshipApplications_MonthlyExpenses", "\"MonthlyExpenses\" >= 0");
        });

        builder.HasKey(application => application.Id);

        builder.Property(application => application.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(application => application.DateOfBirth)
            .IsRequired();

        builder.Property(application => application.MonthlyIncome)
            .HasConversion<MoneyToCentsConverter>()
            .IsRequired();

        builder.Property(application => application.MonthlyExpenses)
            .HasConversion<MoneyToCentsConverter>()
            .IsRequired();

        builder.Property(application => application.HardshipReason)
            .HasMaxLength(1000);

        builder.Property(application => application.CreatedAt)
            .HasConversion<InstantToUtcTicksConverter>()
            .IsRequired();

        builder.Property(application => application.UpdatedAt)
            .HasConversion<InstantToUtcTicksConverter>();

        builder.Ignore(application => application.MonthlySurplus);
        builder.Ignore(application => application.IsInDeficit);

        builder.HasIndex(application => application.CreatedAt);
    }
}
