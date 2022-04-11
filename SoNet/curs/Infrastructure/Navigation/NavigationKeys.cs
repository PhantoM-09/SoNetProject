

namespace curs.Infrastructure.Navigation
{
    public static class NavigationKeys
    {
        //Ключи для раздела с Логином и Регистрацией---------------------------------------------------------------------------------------------------------
        public const string ContextLoginRegisterKey = @"ContextLR";                     //Ключ для View контента Логина и Регистрации
        public const string LoginViewKey = @"LoginViewKey";                             //Ключ для View Логина
        public const string RegisterViewKey = @"RegisterViewKey";                       //Ключ для View Регистрации(фамилия, имя, почта, пароль)
        public const string RegisterEndViewKey = @"RegisterEndViewKey";                 //Ключ для View продолжения Регистрации(пол, дата рождения, страна)
        public const string RegisterPictureViewKey = @"RegisterPictureViewKey";         //Ключ для View конца регистрации(Выбор фото)
        public const string ErrorMessageViewKey = @"ErrorMessageViewKey";               //Ключ для View ошибки в ходе регистрации
        //---------------------------------------------------------------------------------------------------------------------------------------------------

        //Ключи для основоного раздела приложения------------------------------------------------------------------------------------------------------------
        public const string ContextRightInfoViewKey = @"ContextRInfoViewKey";           //Ключ для View контента Правой информации
        public const string AdminContextRightInfoViewKey = @"AdminContextRInfoViewKey"; //Ключ для View контента АДМИНА Правой информации

        //Профиль----------------------------------------------------------------------------------------------------------
        public const string ProfileViewKey = @"ProfileViewKey";                         //Ключ для View страницы Профиля
        public const string AddPostViewKey = @"AddPostViewKey";                         //Ключ для View страницы добавления поста
        public const string AddChangePostViewKey = @"AddChangePostViewKey";             //Ключ для View страницы изменения поста
        public const string DeletePostViewKey = @"DeletePostViewKey";                   //Ключ для View удаления поста
        public const string CommentPostViewKey = @"CommentPostViewKey";                 //Ключ для View комментирования поста
        public const string StrangeProfileViewKey = @"StrangeProfileViewKey";           //Ключ для View страницы чужого Профиля
        //-----------------------------------------------------------------------------------------------------------------
        public const string FriendsViewKey = @"FriendsViewKey";                         //Ключ для View страницы Друзей

        //Сообщения---------------------------------------------------------------------------------------------------------
        public const string MessageViewKey = @"MessageViewKey";                         //Ключ для View страницы Сообщений
        public const string SendMessageViewKey = @"SendMessageViewKey";                 //Ключ для View места Отправки сообщений
        //------------------------------------------------------------------------------------------------------------------

        //Файлы-------------------------------------------------------------------------------------------------------------
        public const string MyFilesViewKey = @"MyFilesViewKey";                         //Ключ для View страницы Файлы
        public const string StrangeFilesViewKey = @"StrangeFilesViewKey";               //Ключ для View страницы чужих Файлов
        //------------------------------------------------------------------------------------------------------------------

        public const string SettingsViewKey = @"SettingsViewKey";                       //Ключ для View страницы Настройки
        public const string SearchResultsViewKey = @"SearchResultsViewKey ";            //Ключ для View страницы Результатов поиска
        public const string ExitViewKey = @"ExitViewKey";                               //Ключ для View страницы Выхода

        public const string DataBaseViewKey = @"DataBaseViewKey";                       //Ключ для View контента Базы данных
        //---------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
