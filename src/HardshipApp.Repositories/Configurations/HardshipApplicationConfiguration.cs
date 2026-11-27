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
            // The database refuses a negative amount even if a future caller bypasses
            // validation. A *deficit* is legitimate and is not constrained here — only a
            // negative income or a negative expense figure, which cannot be meant.
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
            .IsRequired();

        // Derived from the two figures above, so there is nothing to store and nothing that
        // could disagree with them.
        builder.Ignore(application => application.MonthlySurplus);
        builder.Ignore(application => application.IsInDeficit);

        // The list is shown newest first.
        builder.HasIndex(application => application.CreatedAt);
    }
}
