using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.DataModels;
using Application.Exceptions;
using Mapster;
using MediatR;

namespace Application.Budgets.Queries.GetBudgets;

public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, GetBudgetsResponse>
{
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetBudgetsQueryHandler(IBudgetsRepository budgetsRepository, ICurrentUserService currentUserService)
    {
        _budgetsRepository = budgetsRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetBudgetsResponse> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }

        var budgets = await _budgetsRepository.GetCollection(
            x => x.OwnerId == _currentUserService.UserId ||
                 x.SharedBudgets.Any(y => y.UserId == _currentUserService.UserId), cancellationToken);

        var response = new GetBudgetsResponse
        {
            OwnedBudgets = budgets.Where(x => x.OwnerId == _currentUserService.UserId).Adapt<IEnumerable<BudgetDto>>(),
            SharedBudgets = budgets.Where(x => x.OwnerId != _currentUserService.UserId).Adapt<IEnumerable<BudgetDto>>()
        };

        return response;
    }

}