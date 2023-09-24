using System.Reflection;

namespace WebApi.Authorization;

public static class AuthorizationPolicies
{
    public const string CreateCategory = "CreateCategory";
    private static readonly (string PolicyName, string PermissionName) CreateCategoryPolicy = (CreateCategory, "create:category");

    public const string DeleteCategory = "DeleteCategory";
    private static readonly (string PolicyName, string PermissionName) DeleteCategoryPolicy = (DeleteCategory, "delete:category");

    public static IEnumerable<(string PolicyName, string PermissionName)> GetAll()
    {
        var fields = typeof(AuthorizationPolicies).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(x => x.FieldType == typeof((string, string)));

        return fields.Select(x => x.GetValue(null)).Cast<(string, string)>();
    }
}