

namespace curs.Infrastructure.Navigation
{
    public interface INavigationAware
    {
        void WantDoSomethingBeforeClose();                                         //Метод закрывающегося View(последнее желание)
        void WantDoSomethingBeforeOpen(object obj = null);               //Метод открывающегося View(первое желание)
    }
}
