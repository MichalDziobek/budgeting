using Application.Exceptions;
using AutoFixture;
using FluentValidation.Results;

namespace Application.Tests.Unit.Exceptions;

public class ValidationExceptionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_ShouldCreateDictionaryMatchingPassedErrors_WhenSingleErrorPerProperty(int errorCount)
    {
        //Arrange
        var fixture = new Fixture();
        var failures = fixture.CreateMany<ValidationFailure>(errorCount).ToList();
        var expected = failures.ToDictionary(x => x.PropertyName, x => new[] {x.ErrorMessage});

        //Act
        var actual = new ValidationException(failures).Errors;

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_ShouldCreateDictionaryMatchingPassedErrors_WhenMultipleErrorsPerProperty(int errorCount)
    {
        //Arrange
        var fixture = new Fixture();
        var failures = fixture.Build<ValidationFailure>()
            .With(x => x.PropertyName, "Property1").CreateMany(errorCount).ToList();
        failures.AddRange(fixture.Build<ValidationFailure>()
            .With(x => x.PropertyName, "Property2").CreateMany(errorCount));
        var expected = failures.GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage));

        //Act
        var actual = new ValidationException(failures).Errors;

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
}