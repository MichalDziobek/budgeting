using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.EventHandlers;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Domain.Events;
using Mapster;
using Microsoft.Extensions.Logging;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Budgets.EventHandlers;

public class BudgetEntryChangedValueEventHandlerTests
{
    private readonly BudgetEntryChangedValueEventHandler _sut;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly Fixture _fixture;

    public BudgetEntryChangedValueEventHandlerTests()
    {
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _sut = new BudgetEntryChangedValueEventHandler(_budgetsRepository, Substitute.For<ILogger<BudgetEntryChangedValueEventHandler>>());
    }

    [Theory]
    [InlineData(-100, 100,0,200)]
    [InlineData(100, -100,0,-200)]
    [InlineData(100, 100,0,0)]
    [InlineData(-100, -200,-1000,-1100)]
    public async Task ShouldCallUpdateWithCorrectValue_WhenBudgetExists(decimal oldEntryValue, decimal newEntryValue, decimal oldTotalValue, decimal expectedValue)
    {
        //Arrange
        var changedValueEvent = _fixture.Create<BudgetEntryChangedValueEvent>();
        changedValueEvent.OldValue = oldEntryValue;
        changedValueEvent.BudgetEntry.Value = newEntryValue;
        
        var budget = _fixture.Create<Budget>();
        budget.Id = changedValueEvent.BudgetEntry.BudgetId;
        budget.TotalValue = oldTotalValue;

        _budgetsRepository.GetById(changedValueEvent.BudgetEntry.BudgetId, Arg.Any<CancellationToken>())
            .Returns(budget);
        
        //Act
        await _sut.Handle(changedValueEvent, CancellationToken.None);

        //Assert
        await _budgetsRepository.Received(1).Update(Arg.Is<Budget>(x => x.TotalValue == expectedValue && x.Id == budget.Id));
    }
    
    [Fact]
    public async Task ShouldNotCallUpdate_WhenBudgetDoesNotExist()
    {
        //Arrange
        var changedValueEvent = _fixture.Create<BudgetEntryChangedValueEvent>();
        
        var budget = _fixture.Create<Budget>();
        budget.Id = changedValueEvent.BudgetEntry.BudgetId;

        _budgetsRepository.GetById(changedValueEvent.BudgetEntry.BudgetId, Arg.Any<CancellationToken>())
            .ReturnsNull();
        
        //Act
        await _sut.Handle(changedValueEvent, CancellationToken.None);

        //Assert
        await _budgetsRepository.DidNotReceiveWithAnyArgs().Update(Arg.Any<Budget>(), Arg.Any<CancellationToken>());
    }
}