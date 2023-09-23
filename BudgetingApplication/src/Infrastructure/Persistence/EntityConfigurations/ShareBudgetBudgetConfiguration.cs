using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;

public class ShareBudgetBudgetConfiguration : IEntityTypeConfiguration<SharedBudget>
{
    public void Configure(EntityTypeBuilder<SharedBudget> builder)
    {
        builder.HasKey(x => new { x.BudgetId, x.UserId });
        builder.HasOne(x => x.User)
            .WithMany(x => x.SharedBudgets)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Budget)
            .WithMany(x => x.SharedBudgets)
            .HasForeignKey(x => x.BudgetId);
    }
}