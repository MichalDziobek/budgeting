using Xunit;

namespace WebApi.Tests.Integration.Common;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    
}