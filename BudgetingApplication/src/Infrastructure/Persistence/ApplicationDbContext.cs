using System.Reflection;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Budget> Budgets { get; set; } = default!;
    public DbSet<BudgetEntry> BudgetEntries { get; set; } = default!;
    public DbSet<SharedBudget> SharedBudgets { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}