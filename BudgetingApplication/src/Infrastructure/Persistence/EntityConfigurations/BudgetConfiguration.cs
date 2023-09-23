using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

        builder.HasOne(x => x.Owner).WithMany(x => x.OwnedBudgets)
            .HasForeignKey(x => x.OwnerId);

        builder.HasMany(x => x.UsersWithSharedAccess).WithMany(x => x.SharedBudgets);
    }
}