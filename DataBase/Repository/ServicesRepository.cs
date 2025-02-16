﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using USProApplication.Models;
using USProApplication.Models.Repositories;

namespace USProApplication.DataBase.Repository;

public class ServicesRepository(IDbContextFactory<AppDbContext> _contextFactory, IMapper mapper) : IBaseRepository<Models.Service>
{
    public async Task<List<Models.Service>> GetAllAsync()
    {
        await using var context = _contextFactory.CreateDbContext();

        var serivices = await context.Services.ToListAsync();

        return mapper.Map<List<Service>>(serivices);
    }

    public async Task AddAsync(Service service)
    {
        await using var context = _contextFactory.CreateDbContext();

        var entity = mapper.Map<Entities.Service>(service);
        await context.Services.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Service service)
    {
        await using var context = _contextFactory.CreateDbContext();

        var entity = mapper.Map<Entities.Service>(service);
        context.Services.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var entity = await context.Services.FindAsync(id);
        if (entity != null)
        {
            context.Services.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        throw new NotImplementedException();
    }
}
