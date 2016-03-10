using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTimeService.Concrete
{
    public interface IRepository<T> where T:class 
    {
        IQueryable<T> All();

        T Find(int id);

        T Find(string searchStr);

        void Insert(T entity);

        void Update(T entity);

        void Delete(int id);

        void Save();


    }
}
