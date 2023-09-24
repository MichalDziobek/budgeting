using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.Commands.UpdateBudgetEntry;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;

namespace Application.Tests.Unit.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryCommandHandlerTests
{
    private readonly UpdateBudgetEntryCommandHandler _sut;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IBudgetEntriesRepository _budgetEntriesRepository;
    private readonly Fixture _fixture;

    public UpdateBudgetEntryCommandHandlerTests()
    {
        _categoriesRepository = Substitute.For<ICategoriesRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _budgetEntriesRepository = Substitute.For<IBudgetEntriesRepository>();
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();

        _sut = new UpdateBudgetEntryCommandHandler(_categoriesRepository, _currentUserService, _budgetEntriesRepository,
            _budgetsRepository);
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData_ForOwner()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        budget.OwnerId = userId;

        var budgetEntry = _fixture.Create<BudgetEntry>();
        budgetEntry.BudgetId = budget.Id;
        
        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        command.BudgetEntryId = budgetEntry.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetEntriesRepository.GetById(budgetEntry.Id, Arg.Any<CancellationToken>()).Returns(budgetEntry);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetEntriesRepository.Received(1).Update(Arg.Is<BudgetEntry>(x =>
                PropertiesSetCorrectly(x, command, budgetEntry)),
            Arg.Any<CancellationToken>());
    }



    [Fact]
    public async Task ShouldCallCreateWithCorrectData_ForSharedUser()
    {
        //Arrange

        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        budget.SharedBudgets = new List<SharedBudget>
        {
            new()
            {
                UserId = userId,
            }
        };

        var budgetEntry = _fixture.Create<BudgetEntry>();
        budgetEntry.BudgetId = budget.Id;
        
        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        command.BudgetEntryId = budgetEntry.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetEntriesRepository.GetById(budgetEntry.Id, Arg.Any<CancellationToken>()).Returns(budgetEntry);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetEntriesRepository.Received(1).Update(Arg.Is<BudgetEntry>(x =>
                PropertiesSetCorrectly(x, command, budgetEntry)),
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task ShouldReturnCorrectResult()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        budget.OwnerId = userId;

        var budgetEntry = _fixture.Create<BudgetEntry>();
        budgetEntry.BudgetId = budget.Id;
        
        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        command.BudgetEntryId = budgetEntry.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetEntriesRepository.GetById(budgetEntry.Id, Arg.Any<CancellationToken>()).Returns(budgetEntry);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        _budgetEntriesRepository.Update(Arg.Is<BudgetEntry>(x => x.Name == command.Name),
            Arg.Any<CancellationToken>()).Returns(_fixture.Create<BudgetEntry>());
        
        //Act
        var result = await _sut.Handle(command, CancellationToken.None);

        //Assert
        result.BudgetEntry.Id.Should().NotBe(default);
    }
    
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenCategoryDoesNotExistExists()
    {
        //Arrange
        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        var userId = _fixture.Create<string>();

        _currentUserService.UserId.Returns(userId);

        _categoriesRepository.MockExists(new List<Category>());
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenBudgetEntryDoesNotExistExists()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();

        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _categoriesRepository.MockExists(new[] { category });
        _budgetsRepository.MockExists(new List<Budget>());
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task ShouldThrowForbiddenException_WhenUserDoesNotHaveAccessToBudget()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();

        var budgetEntry = _fixture.Create<BudgetEntry>();
        budgetEntry.BudgetId = budget.Id;
        
        var command = _fixture.Create<UpdateBudgetEntryCommand>();
        command.BudgetEntryId = budgetEntry.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetEntriesRepository.GetById(budgetEntry.Id, Arg.Any<CancellationToken>()).Returns(budgetEntry);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
    
    private static bool PropertiesSetCorrectly(BudgetEntry x, UpdateBudgetEntryCommand? command, BudgetEntry? budgetEntry)
    {
        return x.Id == command?.BudgetEntryId &&
               x.Name == command.Name &&
               x.CategoryId == command.CategoryId &&
               x.Value == command.Value &&
               x.BudgetId == budgetEntry?.BudgetId &&
               x.Budget == budgetEntry.Budget && 
               x.Category == budgetEntry.Category;
    }
}