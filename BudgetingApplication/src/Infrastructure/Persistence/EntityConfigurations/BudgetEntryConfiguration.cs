using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class BudgetEntryConfiguration : IEntityTypeConfiguration<BudgetEntry>
{
    public void Configure(EntityTypeBuilder<BudgetEntry> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

        builder.HasOne(x => x.Budget).WithMany(x => x.BudgetEntries)
            .HasForeignKey(x => x.BudgetId).OnDelete(DeleteBehavior.Cascade);
    }
}