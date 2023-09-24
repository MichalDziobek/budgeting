using Application.Abstractions.Persistence;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Budgets.EventHandlers;

public class BudgetEntryChangedValueEventHandler : INotificationHandler<BudgetEntryChangedValueEvent>
{
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly ILogger<BudgetEntryChangedValueEventHandler> _logger;

    public BudgetEntryChangedValueEventHandler(IBudgetsRepository budgetsRepository, ILogger<BudgetEntryChangedValueEventHandler> logger)
    {
        _budgetsRepository = budgetsRepository;
        _logger = logger;
    }

    public async Task Handle(BudgetEntryChangedValueEvent notification, CancellationToken cancellationToken)
    {
        var budget = await _budgetsRepository.GetById(notification.BudgetEntry.BudgetId, cancellationToken);
        if (budget is null)
        {
            _logger.LogError("Received BudgetEntryChangedValueEvent with incorrect BudgetId");
            return;
        }

        budget.TotalValue = budget.TotalValue - notification.OldValue + notification.BudgetEntry.Value;
        await _budgetsRepository.Update(budget, cancellationToken);
    }
}