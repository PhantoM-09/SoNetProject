using System;

namespace curs.Infrastructure.Commands.Base
{
    public class DelegateCommand : BaseForBaseCommand
    {
        private readonly Action<object> execute;                    //Выполняемая команда
        private readonly Func<object, bool> canExecute;             //Можно ли выполнить команду

        #region Ctors

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new NullReferenceException(nameof(execute));    //Если выполняемой команды нет - бросаем исключение
            this.canExecute = canExecute;                                                   //В то же время, разрешение на выполнение может отсутствовать и тогда команда по умолчанию выполняется
        }

        #endregion

        #region Methods
        public override bool CanExecute(object parameter) => this.canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => this.execute(parameter);

        #endregion
    }
}
