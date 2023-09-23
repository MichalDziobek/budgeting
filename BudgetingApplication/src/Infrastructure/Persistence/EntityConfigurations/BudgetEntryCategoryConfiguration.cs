using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class BudgetEntryCategoryConfiguration : IEntityTypeConfiguration<BudgetEntryCategory>
{
    public void Configure(EntityTypeBuilder<BudgetEntryCategory> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
    }
}