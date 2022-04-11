using curs.Infrastructure.Navigation;
using curs.ServiceChat;
using curs.View.LoginRegistrationView;
using curs.View.MainElementView;
using curs.View.RightInformationView;
using curs.View.RightInformationView.Admin;
using curs.ViewModels.LoginRegistration;
using curs.ViewModels.MainElements;
using curs.ViewModels.RightInformation;
using DatabaseClasses.UnitOfWorkPattern;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace curs
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application, IServiceChatCallback
    {
        NavigationManager navigationManager;

        public void MessageCallBack(string message, string idUser)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var window = new MainWindowView();
            navigationManager = new NavigationManager(Dispatcher, window.UserControlsContent);

            navigationManager.Register(NavigationKeys.ContextLoginRegisterKey);
            navigationManager.Register(NavigationKeys.ContextRightInfoViewKey);
            navigationManager.Register(NavigationKeys.AdminContextRightInfoViewKey);
            

            var viewModel = new MainWindowViewModel(navigationManager);
            window.DataContext = viewModel;

            Application.Current.MainWindow.Closing += MainWindow_Closing;

            window.Show();
        }


        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (navigationManager != null)
                {
                    using (UnitOfWork unit = new UnitOfWork())
                    {
                        string email = navigationManager.EmailCurrentUser;
                        var user = unit.UserDataRepos.GetItem(new object[] { email });          //Достаем юзера, который сейчас в сети
                        user.IsOnline = false;                                                  //Меняем ему статус,  как он выходит

                        unit.UserDataRepos.UpdateItem(user);
                        unit.Save();

                        var friends = unit.FriendRepos.GetItems().Where(f => f.UserEmail == email).Select(u => u);

                        ServiceChatClient client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                        foreach (var friend in friends)
                        {

                            client.Disconnect(email, friend.FriendEmail);
                        }
                        
                    }

                  
            }
            }
            catch
            {

            }
           
            
        }
    }
}
