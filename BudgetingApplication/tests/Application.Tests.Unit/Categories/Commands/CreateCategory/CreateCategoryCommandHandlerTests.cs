using Application.Abstractions.Persistence;
using Application.Categories.Commands.CreateCategoryCommand;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;

namespace Application.Tests.Unit.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly CreateCategoryCommandHandler _sut;
    private readonly IBudgetEntryCategoriesRepository _categoriesRepository;

    public CreateCategoryCommandHandlerTests()
    {
        _categoriesRepository = Substitute.For<IBudgetEntryCategoriesRepository>();
        _sut = new CreateCategoryCommandHandler(_categoriesRepository);
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData()
    {
        //Arrange
        var fixture = new Fixture();
        var command = fixture.Create<CreateCategoryCommand>();
        var userId = fixture.Create<string>();
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _categoriesRepository.Received(1).Create(Arg.Is<Category>(x => x.Name == command.Name));
    }
    
    [Fact]
    public async Task ShouldReturnCorrectResult()
    {
        //Arrange
        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        var command = fixture.Create<CreateCategoryCommand>();
        var userId = fixture.Create<string>();
        var budget = fixture.Create<Category>();

        _categoriesRepository.Create(Arg.Is<Category>(x => x.Name == command.Name),
            Arg.Any<CancellationToken>()).Returns(budget);
        
        //Act

        var result = await _sut.Handle(command, CancellationToken.None);

        //Assert
        result.Category.Id.Should().NotBe(default);
    }
    
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenCategoryExists()
    {
        //Arrange
        var fixture = new Fixture();
        var command = fixture.Create<CreateCategoryCommand>();
        var category = command.Adapt<Category>();

        _categoriesRepository.MockExists(new []{ category });
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}