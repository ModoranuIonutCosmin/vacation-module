using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacationsModule.Application.Interfaces;
using VacationsModule.Application.Interfaces.Repositories;
using VacationsModule.Application.Interfaces.Repositories.Base;
using VacationsModule.Domain.Seedwork;

namespace VacationsModule.Infrastructure.Data_Access.Base;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    // where TEntity : Entity 
    where TEntity : class 
{
    private readonly   ILogger<Repository<TEntity, TKey>> _logger;
    protected readonly VacationRequestsDBContext          _context;
    private readonly   IUnitOfWork                        _unitOfWork;

    public Repository(VacationRequestsDBContext context, ILogger<Repository<TEntity, TKey>> logger,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _logger          = logger;
        _unitOfWork      = unitOfWork;

        _unitOfWork.Register(this);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(List<TEntity> entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllWhereAsync
        (Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>()
                                     .Where(predicate).ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await _context.FindAsync<TEntity>(id);
    }

    public async Task DeleteWhereAsync(Func<TEntity, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        _context.RemoveRange(_context.Set<TEntity>().Where(predicate));

        await _context.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task Submit()
    {
        await _context.SaveChangesAsync();
    }
}