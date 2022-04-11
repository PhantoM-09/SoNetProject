using System.Collections.Generic;

namespace DatabaseClasses.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //Получить всю коллекцию
        IEnumerable<TEntity> GetItems();

        //Получить элемент по первичному ключу
        TEntity GetItem(object[] keys);

        //Добавление элемента в таблицу
        void AddItem(TEntity item);

        //Обновить элемент
        void UpdateItem(TEntity item);

        //Удалить элемент по первичному ключу
        void DeleteItem(object[] keys);
    }
}
