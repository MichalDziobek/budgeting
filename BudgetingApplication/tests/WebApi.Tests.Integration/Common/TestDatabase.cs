using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Respawn;
using WebApi.Tests.Integration.Common.Abstractions;

namespace WebApi.Tests.Integration.Common;

public class TestDatabase : ITestDatabase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Respawner _respawner;

    public TestDatabase(ApplicationDbContext dbContext, Respawner respawner)
    {
        _dbContext = dbContext;
        _respawner = respawner;
    }

    public async Task<TEntity?> FindAsync<TEntity, TId>(TId id)
        where TEntity : class
    {
        return await _dbContext.FindAsync<TEntity>(id);
    }

    public async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task ResetAsync()
    {
        await using var dbConnection = _dbContext.Database.GetDbConnection();
        await dbConnection.OpenAsync();
        
        await _respawner.ResetAsync(dbConnection);
        await dbConnection.CloseAsync();
    }
}