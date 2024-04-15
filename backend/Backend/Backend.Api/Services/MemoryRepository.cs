using Backend.Api.Models;
using System.Linq;

namespace Backend.Api.Services
{
    public interface IRepository
    {
        T? Get<T>(string id) where T : ModelBase;
        IList<T> GetAll<T>();
        IQueryable<T> Query<T>();
        void Insert<T>(T entity);
    }

    public class MemoryRepository : IRepository
    {
        readonly IList<object> _entities = new List<object>();

        public T? Get<T>(string id) where T : ModelBase
        {
            return _entities.OfType<T>().SingleOrDefault(e => e.Id == id);
        }

        public IList<T> GetAll<T>()
        {
            return _entities.OfType<T>().ToList();
        }

        public IQueryable<T> Query<T>()
        {
            return GetAll<T>().AsQueryable();
        }

        public void Insert<T>(T entity)
        {
            _entities.Add(entity);
        }
    }
}
