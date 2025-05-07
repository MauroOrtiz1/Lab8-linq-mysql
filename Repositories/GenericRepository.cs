using Microsoft.EntityFrameworkCore;
using Lab8_BernieOrtiz.Models;
namespace Lab8_BernieOrtiz.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class {
    private readonly dbcontext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(dbcontext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public Task<IEnumerable<T>> GetAllAsync() {
        return Task.FromResult(_dbSet.AsEnumerable());
    }
    public Task<T?> GetByIdAsync(object id) {
        return _dbSet.FindAsync(id).AsTask();
    }
    public Task InsertAsync(T entity) {
        _dbSet.Add(entity);
        return _context.SaveChangesAsync();
    }
    public void InsertWithoutSave(T entity) {
        _dbSet.Add(entity);
    }
    public Task AddAsync(T entity) {
        _dbSet.Add(entity);
        return Task.CompletedTask;
    }
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
    public Task SaveAsync() {
        return _context.SaveChangesAsync();
    }
}