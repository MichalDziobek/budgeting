using AutoFixture;

namespace Application.Tests.Unit.Extensions;

public static class AutoFixtureExtensions
{
    public static Fixture ChangeToOmitOnRecursionBehaviour(this Fixture fixture)
    {
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}