using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagerWpf.Services
{
    public interface ICrudService<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
