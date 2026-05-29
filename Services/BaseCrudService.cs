using HotelManagerWpf.Data;
using HotelManagerWpf.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HotelManagerWpf.Services
{
    public class BaseCrudService<T> : ICrudService<T> where T : BaseEntity
    {
        public virtual async Task<List<T>> GetAllAsync()
        {
            using var context = new AppDbContext();
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            using var context = new AppDbContext();
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task<bool> CreateAsync(T entity)
        {
            try
            {
                using var context = new AppDbContext();
                context.Set<T>().Add(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                using var context = new AppDbContext();
                context.Set<T>().Update(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var context = new AppDbContext();
                var entity = await context.Set<T>().FindAsync(id);
                if (entity == null) return false;
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
