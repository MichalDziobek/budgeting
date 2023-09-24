using Domain.Entities;
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
        where TId : notnull
        where TEntity : BaseEntity<TId>
    {
        //Clear is sometimes required when we create and retrieve data in test, while modifying it in application
        _dbContext.ChangeTracker.Clear(); 
        return await _dbContext.FindAsync<TEntity>(id);
    }

    public async Task<TEntity> AddAsync<TEntity, TId>(TEntity entity)
        where TId : notnull
        where TEntity : BaseEntity<TId>
    {
        _dbContext.Add(entity);

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync<TEntity, TId>(IEnumerable<TEntity> entity)
        where TEntity : BaseEntity<TId>
        where TId : notnull
    {
        _dbContext.AddRange(entity);

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