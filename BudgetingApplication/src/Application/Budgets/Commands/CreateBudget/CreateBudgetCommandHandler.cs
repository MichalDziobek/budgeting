using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Exceptions;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, CreateBudgetResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;

    public CreateBudgetCommandHandler(ICurrentUserService currentUserService, IBudgetsRepository budgetsRepository)
    {
        _currentUserService = currentUserService;
        _budgetsRepository = budgetsRepository;
    }

    public async Task<CreateBudgetResponse> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }

        if (await _budgetsRepository.Exists(x => x.OwnerId == _currentUserService.UserId && x.Name == request.Name,
                cancellationToken))
        {
            throw new BadRequestException("This user already exists");
        }

        var budget = request.Adapt<Budget>();
        budget.OwnerId = _currentUserService.UserId;

        var createdBudget = await _budgetsRepository.Create(budget, cancellationToken);
        var response = new CreateBudgetResponse
        {
            Budget = createdBudget.Adapt<BudgetDto>()
        };

        return response;
    }
}