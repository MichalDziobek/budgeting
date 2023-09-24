using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.DataModel;
using Application.Exceptions;
using Mapster;
using MediatR;

namespace Application.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryCommandHandler : IRequestHandler<UpdateBudgetEntryCommand, UpdateBudgetEntryResponse>
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IBudgetEntriesRepository _budgetEntriesRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBudgetEntryCommandHandler(ICategoriesRepository categoriesRepository,
        ICurrentUserService currentUserService, IBudgetEntriesRepository budgetEntriesRepository,
        IBudgetsRepository budgetsRepository)
    {
        _categoriesRepository = categoriesRepository;
        _currentUserService = currentUserService;
        _budgetEntriesRepository = budgetEntriesRepository;
        _budgetsRepository = budgetsRepository;
    }

    public async Task<UpdateBudgetEntryResponse> Handle(UpdateBudgetEntryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }
        
        if (!await _categoriesRepository.Exists(x => x.Id == request.CategoryId, cancellationToken))
        {
            throw new BadRequestException("Requested category does not exist");
        }

        var existingBudgetEntry = await _budgetEntriesRepository.GetById(request.BudgetEntryId, cancellationToken);

        if (existingBudgetEntry is null)
        {
            throw new NotFoundException("Requested budget entry does not exist");

        }
        
        if (!await _budgetsRepository.Exists(
                x => x.Id == existingBudgetEntry.BudgetId &&
                    (x.OwnerId == _currentUserService.UserId ||
                    x.SharedBudgets.Any(y => y.UserId == _currentUserService.UserId)), cancellationToken))
        {
            throw new ForbiddenException("Current user does not have access to requested budget");
        }

        request.Adapt(existingBudgetEntry);

        var updatedBudgetEntry = await _budgetEntriesRepository.Update(existingBudgetEntry, cancellationToken);
        var response = new UpdateBudgetEntryResponse()
        {
            BudgetEntry = updatedBudgetEntry.Adapt<BudgetEntryDto>()
        };

        return response;
    }
}