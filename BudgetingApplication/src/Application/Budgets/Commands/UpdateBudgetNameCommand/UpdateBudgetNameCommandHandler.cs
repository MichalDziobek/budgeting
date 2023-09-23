using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Exceptions;
using MediatR;

namespace Application.Budgets.Commands.UpdateBudgetNameCommand;

public class UpdateBudgetNameCommandHandler : IRequestHandler<UpdateBudgetNameCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;

    public UpdateBudgetNameCommandHandler(ICurrentUserService currentUserService, IBudgetsRepository budgetsRepository)
    {
        _currentUserService = currentUserService;
        _budgetsRepository = budgetsRepository;
    }

    public async Task Handle(UpdateBudgetNameCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }

        var existingBudget = await _budgetsRepository.GetById(request.BudgetId, cancellationToken);

        if (existingBudget is null)
        {
            throw new NotFoundException("Budget with requested Id does not exist");
        }

        if (existingBudget.OwnerId != _currentUserService.UserId)
        {
            throw new ForbiddenException("Only budget owner can rename budgets");
        }

        existingBudget.Name = request.Name;
        await _budgetsRepository.Update(existingBudget, cancellationToken);
    }
}