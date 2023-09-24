namespace WebApi.Tests.Integration.Common;

public interface ITestPermissionsProvider
{
    public IEnumerable<string> GetPermissionValues();
}