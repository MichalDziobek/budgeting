using Application.Abstractions.Persistence;
using Application.Categories.Commands.CreateCategoryCommand;
using Application.Categories.Commands.DeleteCategory;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly DeleteCategoryCommandHandler _sut;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly Fixture _fixture;

    public DeleteCategoryCommandHandlerTests()
    {
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _categoriesRepository = Substitute.For<ICategoriesRepository>();
        _sut = new DeleteCategoryCommandHandler(_categoriesRepository);
    }

    [Fact]
    public async Task ShouldCallDeleteWithCorrectData()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var command = new DeleteCategoryCommand() { CategoryId = category.Id };
        _categoriesRepository.GetById(command.CategoryId, Arg.Any<CancellationToken>()).Returns(category);
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _categoriesRepository.Received(1).Delete(Arg.Is<Category>(x => x.Id == command.CategoryId));
    }

    [Fact]
    public async Task ShouldThrowBadRequestException_WhenCategoryDoesNotExist()
    {
        //Arrange
        var category = _fixture.Create<Category>();
        var command = new DeleteCategoryCommand() { CategoryId = category.Id + 1 };
        _categoriesRepository.GetById(command.CategoryId, Arg.Any<CancellationToken>()).ReturnsNull();
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}