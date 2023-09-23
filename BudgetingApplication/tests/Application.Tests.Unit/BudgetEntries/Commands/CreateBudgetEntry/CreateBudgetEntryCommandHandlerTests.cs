using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;

namespace Application.Tests.Unit.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryCommandHandlerTests
{
    private readonly CreateBudgetEntryCommandHandler _sut;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IBudgetEntriesRepository _budgetEntriesRepository;
    private readonly Fixture _fixture;

    public CreateBudgetEntryCommandHandlerTests()
    {
        _categoriesRepository = Substitute.For<ICategoriesRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _budgetEntriesRepository = Substitute.For<IBudgetEntriesRepository>();
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();

        _sut = new CreateBudgetEntryCommandHandler(_categoriesRepository, _currentUserService, _budgetEntriesRepository,
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
        
        var command = _fixture.Create<CreateBudgetEntryCommand>();
        command.BudgetId = budget.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetEntriesRepository.Received(1).Create(Arg.Is<BudgetEntry>(x => x.Name == command.Name));
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
            new SharedBudget
            {
                UserId = userId,
            }
        };
        
        var command = _fixture.Create<CreateBudgetEntryCommand>();
        command.BudgetId = budget.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetEntriesRepository.Received(1).Create(Arg.Is<BudgetEntry>(x => x.Name == command.Name));
    }
    
    [Fact]
    public async Task ShouldReturnCorrectResult()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        budget.OwnerId = userId;
        
        var command = _fixture.Create<CreateBudgetEntryCommand>();
        command.BudgetId = budget.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        _budgetEntriesRepository.Create(Arg.Is<BudgetEntry>(x => x.Name == command.Name),
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
        var command = _fixture.Create<CreateBudgetEntryCommand>();
        var userId = _fixture.Create<string>();

        _currentUserService.UserId.Returns(userId);

        _categoriesRepository.MockExists(new List<Category>());
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenBudgetDoesNotExistExists()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var userId = _fixture.Create<string>();

        var command = _fixture.Create<CreateBudgetEntryCommand>();
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
        
        var command = _fixture.Create<CreateBudgetEntryCommand>();
        command.BudgetId = budget.Id;
        command.CategoryId = category.Id;
        
        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.MockExists(new[] { budget });
        _categoriesRepository.MockExists(new[] { category });
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}