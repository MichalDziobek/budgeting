using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Budgets.Commands.ShareBudgetCommand;

public class ShareBudgetCommandHandler : IRequestHandler<ShareBudgetCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IUsersRepository _usersRepository;

    public ShareBudgetCommandHandler(ICurrentUserService currentUserService, IBudgetsRepository budgetsRepository, IUsersRepository usersRepository)
    {
        _currentUserService = currentUserService;
        _budgetsRepository = budgetsRepository;
        _usersRepository = usersRepository;
    }

    public async Task Handle(ShareBudgetCommand request, CancellationToken cancellationToken)
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

        var existingUser = await _usersRepository.GetById(request.SharedUserId, cancellationToken);
        
        if (existingUser is null)
        {
            throw new BadRequestException("User with requested Id does not exist");
        }

        if (existingBudget.OwnerId != _currentUserService.UserId)
        {
            throw new ForbiddenException("Only budget owner can rename budgets");
        }

        var sharedBudgetEntry = new SharedBudget
        {
            UserId = existingUser.Id,
            BudgetId = existingBudget.Id
        };
        await _budgetsRepository.AddSharedBudget(sharedBudgetEntry, cancellationToken);
    }
}