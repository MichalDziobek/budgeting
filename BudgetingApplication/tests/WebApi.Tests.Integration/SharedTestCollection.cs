using Xunit;

namespace WebApi.Tests.Integration;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    
}