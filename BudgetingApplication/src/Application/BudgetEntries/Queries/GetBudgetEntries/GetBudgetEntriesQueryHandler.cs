using System.Linq.Expressions;
using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.DataModel;
using Application.DataModels.Common;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesQueryHandler : IRequestHandler<GetBudgetEntriesQuery, GetBudgetEntriesResponse>
{
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IBudgetEntriesRepository _budgetEntriesRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBudgetEntriesQueryHandler(IBudgetsRepository budgetsRepository,
        IBudgetEntriesRepository budgetEntriesRepository, ICurrentUserService currentUserService)
    {
        _budgetsRepository = budgetsRepository;
        _budgetEntriesRepository = budgetEntriesRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetBudgetEntriesResponse> Handle(GetBudgetEntriesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }

        var budget = await _budgetsRepository.GetById(request.BudgetId, cancellationToken);
        if (budget is null)
        {
            throw new NotFoundException("Budget with requested Id does not exist");
        }
        if (budget.OwnerId != _currentUserService.UserId && budget.SharedBudgets.All(x => x.UserId != _currentUserService.UserId))
        {
            throw new ForbiddenException("Current user does not have access to requested budget data");
        }
        
        var filters = GetFilters(request);

        var budgetEntriesDtos =
            (await _budgetEntriesRepository.GetPaginatedResponse(request.Offset, request.Limit, filters,
                cancellationToken));

        var paginatedResponse = PaginatedResponse<BudgetEntryDto>.AdaptFromPaginatedResult(budgetEntriesDtos);
        var response = new GetBudgetEntriesResponse()
        {
            BudgetEntries = paginatedResponse
        };
        return response;
    }

    private static Func<IQueryable<BudgetEntry>, IQueryable<BudgetEntry>> GetFilters(GetBudgetEntriesQuery request)
    {
        Func<IQueryable<BudgetEntry>, IQueryable<BudgetEntry>> filters = budgetEntries =>
            budgetEntries.Where(x => x.BudgetId == request.BudgetId);
        if (request.BudgetEntryTypeFilter is not null)
        {
            Expression<Func<BudgetEntry, bool>> predicate = request.BudgetEntryTypeFilter switch
            {
                BudgetEntryType.Income => x => x.Value > 0,
                BudgetEntryType.Expense => x => x.Value < 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            var filtersCopy = filters;
            filters = budgetEntries => filtersCopy(budgetEntries).Where(predicate);
        }

        if (request.CategoryFilter is not null)
        {
            var filtersCopy = filters;
            filters = budgetEntries => filtersCopy(budgetEntries).Where(x => x.CategoryId == request.CategoryFilter);
        }

        return filters;
    }
}