using System.ServiceModel;


namespace NetClassLibrary
{
    public class ServiceUser
    {
        public string ID { get; set; }                 //ID юзера/клиента

        public OperationContext operationContext { get; set; }      //Информация о подключении клиента к сервису
    }
}
