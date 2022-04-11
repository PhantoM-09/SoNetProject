using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NetClassLibrary
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServiceChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        void Connect(string idUser, string idFriend);                  //Метод для подключения клиента к сервису; возвращает id клиента; получает имя подключаемого юзера/клиента

        [OperationContract]
        void Disconnect(string idUser, string idFriend);        //Метод для отключения клиента от сервиса; принимает id клиента

        //[OperationContract(IsOneWay = true)]    //IsOneWay для того, чтобы не дожидаться ответа от сервиса
        //void SendMessage(string message, string idUser, int postId);       //Метод для отправки сообщения на сервис; принимает строку сообщения
        [OperationContract(IsOneWay = true)]
        void SendMessageToChat(string message, string idUser, string idFriend);
    }

    public interface IServiceChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MessageCallBack(string message, string idUser);       //Метод для вызова на стороне клиента со стороны сервиса. Реализация на стороне клиента
    }

}
