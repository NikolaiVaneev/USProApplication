using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading;
using USProApplication.DataBase.Entities;
using USProApplication.Models;

namespace USProApplication.DataBase.Repository;

using Service = Entities.Service;

public class ServicesRepository(IDbContextFactory<AppDbContext> contextFactory) : IBaseRepository<Service>
{
    public async Task AddAsync(Service entity)
    {
        using var _context = await contextFactory.CreateDbContextAsync();

        entity.Id = Guid.NewGuid();
        _context.Services.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var _context = await contextFactory.CreateDbContextAsync();

        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }
    }

    public async Task<List<Service>> GetAllAsync()
    {
        using var _context = await contextFactory.CreateDbContextAsync();
        return await _context.Services.ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        using var _context = await contextFactory.CreateDbContextAsync();
        return await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(Service entity)
    {
        using var _context = await contextFactory.CreateDbContextAsync();

        var existingService = await _context.Services.FindAsync(entity.Id);
        if (existingService != null)
        {
            _context.Entry(existingService).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");
        }
    }
}
