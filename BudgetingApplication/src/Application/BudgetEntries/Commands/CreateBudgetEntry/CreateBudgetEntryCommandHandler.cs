using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.DataModel;
using Application.Exceptions;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryCommandHandler : IRequestHandler<CreateBudgetEntryCommand, CreateBudgetEntryResponse>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IBudgetEntriesRepository _budgetEntriesRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateBudgetEntryCommandHandler(ICategoriesRepository categoriesRepository,
        ICurrentUserService currentUserService, IBudgetEntriesRepository budgetEntriesRepository,
        IBudgetsRepository budgetsRepository)
    {
        _categoriesRepository = categoriesRepository;
        _currentUserService = currentUserService;
        _budgetEntriesRepository = budgetEntriesRepository;
        _budgetsRepository = budgetsRepository;
    }

    public async Task<CreateBudgetEntryResponse> Handle(CreateBudgetEntryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }
        
        if (!await _categoriesRepository.Exists(x => x.Id == request.CategoryId, cancellationToken))
        {
            throw new BadRequestException("Requested category does not exist");
        }
        
        if (!await _budgetsRepository.Exists(x => x.Id == request.BudgetId, cancellationToken))
        {
            throw new NotFoundException("Requested budget does not exist");
        }
        
        if (!await _budgetsRepository.Exists(
                x => x.Id == request.BudgetId &&
                    (x.OwnerId == _currentUserService.UserId ||
                    x.SharedBudgets.Any(y => y.UserId == _currentUserService.UserId)), cancellationToken))
        {
            throw new ForbiddenException("Current user does not have access to requested budget");
        }

        var budgetEntry = request.Adapt<BudgetEntry>();

        var createdBudgetEntry = await _budgetEntriesRepository.Create(budgetEntry, cancellationToken);
        var response = new CreateBudgetEntryResponse()
        {
            BudgetEntry = createdBudgetEntry.Adapt<BudgetEntryDto>()
        };

        return response;
    }
}