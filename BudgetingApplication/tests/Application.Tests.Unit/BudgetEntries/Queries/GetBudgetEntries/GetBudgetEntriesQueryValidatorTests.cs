using Application.BudgetEntries.Queries.GetBudgetEntries;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesQueryValidatorTests
{
    private readonly GetBudgetEntriesQueryValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenMinimalData()
    {
        //Arrange
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = 1,
            Offset = 0,
            Limit = 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    
    [Fact]
    public void ShouldFail_NegativeOffset()
    {
        //Arrange
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = 1,
            Offset = -1,
            Limit = 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Offset);
    }
    
    [Fact]
    public void ShouldFail_EmptyBudgetId()
    {
        //Arrange
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = default,
            Offset = 0,
            Limit = 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetId);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ShouldFail_EmptyIncorrectLimit(int limit)
    {
        //Arrange
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = default,
            Offset = 0,
            Limit = limit,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

}