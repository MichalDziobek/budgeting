using Application.Abstractions.Persistence;
using Application.Categories.Commands.CreateCategory;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;

namespace Application.Tests.Unit.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly CreateCategoryCommandHandler _sut;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly Fixture _fixture;

    public CreateCategoryCommandHandlerTests()
    {
        _categoriesRepository = Substitute.For<ICategoriesRepository>();
        _sut = new CreateCategoryCommandHandler(_categoriesRepository);
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData()
    {
        //Arrange
        var command = _fixture.Create<CreateCategoryCommand>();
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _categoriesRepository.Received(1).Create(Arg.Is<Category>(x => x.Name == command.Name));
    }
    
    [Fact]
    public async Task ShouldReturnCorrectResult()
    {
        //Arrange
        var command = _fixture.Create<CreateCategoryCommand>();
        var budget = _fixture.Create<Category>();

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
        var command = _fixture.Create<CreateCategoryCommand>();
        var category = command.Adapt<Category>();

        _categoriesRepository.MockExists(new []{ category });
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}